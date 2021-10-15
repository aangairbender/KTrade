using System;
using System.Collections.Generic;
using System.Linq;

namespace KTrade.Core
{
    public class GameSession
    {
        public Dictionary<Player, PlayerData> PlayersData { get; } = new Dictionary<Player, PlayerData>();
        public List<Order> Orders { get; } = new List<Order>();
        public List<Trade> Trades { get; } = new List<Trade>();
        public Dictionary<Resource, int> AuctionAmounts { get; } = new Dictionary<Resource, int>();

        public GameConfig Config { get; }

        public int CurrentTick { get; private set; }

        public GameSession(ICollection<Player> players, GameConfig config)
        {
            CurrentTick = 0;
            Config = config;
            Resource.SetBaseAmount(Config.BaseResourcesAmount);

            foreach (var player in players)
            {
                PlayersData.Add(player, PlayerData.Create(Config));
                player.PlayerController.UpgradeStorageCalled += UpgradeStorage;
                player.PlayerController.UpdateAuctionPaymentsCalled += UpdateAuctionPayments;
                player.PlayerController.AcceptOrderCalled += AcceptOrder;
                player.PlayerController.AcceptTradeCalled += AcceptTrade;
                player.PlayerController.PlaceTradeCalled += PlaceTrade;
            }

            for (var i = 0; i < Resource.TotalAmount; ++i)
                AuctionAmounts.Add(Resource.From(i), 0);


            SetupAuction();
        }

        public bool GameFinished { get; set; } = false;

        public void Tick()
        {
            if (GameFinished)
                return;
            
            CurrentTick += 1;

            var maxOrderAmount = (int) (PlayersData.Count * Config.OrdersAmountFromPlayerAmountMultiplier);
            while (Orders.Count < maxOrderAmount)
            {
                var newOrder = Order.Create(Config, CurrentTick + Config.AuctionCooldown);
                Orders.Add(newOrder);
                foreach (var player in PlayersData.Keys)
                {
                    player.PlayerControllerCallback.NewOrderAppeared(newOrder);
                }
            }

            foreach (var trade in Trades.ToList())
            {
                trade.Tick();
                if (trade.TimeLeft == 0)
                {
                    Trades.Remove(trade);
                    ReturnTrade(trade);
                }
            }

            if (CurrentTick % Config.AuctionCooldown == 0)
            {
                DoAuction();
                SetupAuction();
            }

            if (CurrentTick % Config.BonusInterval == 0)
            {
                foreach (var playerData in PlayersData.Values)
                {
                    playerData.Money += Config.BonusAmount;
                }
            }

            foreach (var player in PlayersData.Keys)
            {
                player.PlayerControllerCallback.GameSessionUpdated(GeneratePlayerGameSessionFor(player));
            }

            foreach (var player in PlayersData.Keys)
            {
                if (PlayersData[player].Money < Config.MoneyForWin)
                    continue;

                GameFinished = true;
                foreach (var player2 in PlayersData.Keys)
                {
                    player2.PlayerControllerCallback.PlayerWon(player);
                }

                break;
            }
        }

        private void SetupAuction()
        {
            var nextAuctionTick = (CurrentTick + 1 + Config.AuctionCooldown - 1) / Config.AuctionCooldown *
                                  Config.AuctionCooldown;
            for (var i = 0; i < Resource.TotalAmount; ++i)
            {
                var resource = Resource.From(i);
                var maxIncome = (int) (PlayersData.Count * Config.ResourceAuctionFromTickMultiplier * nextAuctionTick /
                                       resource.RarityTickDivisor) + 1;
                var minIncome = 0;//maxIncome / 2;
                var income = Config.Random.Next(minIncome, maxIncome);
                AuctionAmounts[resource] = income;
            }
        }

        private void DoAuction()
        {
            var playerIncomes = new Dictionary<Player, Dictionary<Resource, int>>();
            foreach (var player in PlayersData.Keys)
            {
                playerIncomes.Add(player, new Dictionary<Resource, int>());
            }

            for (var i = 0; i < Resource.TotalAmount; ++i)
            {
                var resource = Resource.From(i);
                var income = AuctionAmounts[resource];

                var playersBets = PlayersData
                    .ToDictionary(pd => pd.Key, pd => pd.Value.AuctionPayments[resource])
                    .ToList()
                    .OrderBy(kvp => kvp.Value)
                    .ToList();

                var totalBet = playersBets.Sum(kvp => kvp.Value);
                if (totalBet == 0)
                    continue;

                foreach (var kvp in playersBets)
                {
                    var player = kvp.Key;
                    var bet = kvp.Value;

                    var playerIncome = income * bet / totalBet;
                    PlayersData[player].Storage.Add(resource, playerIncome);
                    playerIncomes[player].Add(resource, playerIncome);
                }
            }

            foreach (var playerData in PlayersData.Values)
            {
                for (var i = 0; i < Resource.TotalAmount; ++i)
                    playerData.AuctionPayments[Resource.From(i)] = 0;
            }

            foreach (var player in PlayersData.Keys)
            {
                player.PlayerControllerCallback.AuctionDone(playerIncomes[player]);
            }
        }

        private PlayerGameSession GeneratePlayerGameSessionFor(Player player)
        {
            return new PlayerGameSession
            {
                GameConfig = Config,
                CurrentTick = CurrentTick,
                Player = player,
                PlayerData = PlayersData[player],
                Trades = Trades.ToList(),
                Orders = Orders.ToList(),
                AuctionAmounts = AuctionAmounts
            };
        }

        private void UpdateAuctionPayments(Player player, Dictionary<Resource, int> payments)
        {
            var playerData = PlayersData[player];
            var currentTotal = playerData.AuctionPayments.Values.Sum();
            var newTotal = payments.Values.Sum();

            if (playerData.Money + currentTotal - newTotal < 0)
            {
                player.PlayerControllerCallback.Failure(FailureMessage.NotEnoughMoney);
                return;
            }

            foreach (var kvp in payments)
            {
                var resource = kvp.Key;
                var moneyPaid = kvp.Value;

                playerData.Money += playerData.AuctionPayments[resource];
                playerData.Money -= moneyPaid;

                playerData.AuctionPayments[resource] = moneyPaid;
            }
            player.PlayerControllerCallback.Success();
        }

        private void ReturnTrade(Trade trade)
        {
            var playerData = PlayersData[trade.Author];
            foreach (var kvp in trade.GivenResources)
            {
                var resource = kvp.Key;
                var amount = kvp.Value;

                playerData.Storage.Add(resource, amount);
            }
            playerData.Money += trade.GivingMoney;
            trade.Author.PlayerControllerCallback.PlacedTradeReturned(trade);
        }

        private void UpgradeStorage(Player player)
        {
            var playerData = PlayersData[player];
            var upgradeable = playerData.Storage;

            var cost = upgradeable.UpgradeCost;
            if (cost > playerData.Money)
            {
                player.PlayerControllerCallback.Failure(FailureMessage.NotEnoughMoney);
                return;
            }

            upgradeable.Upgrade();
            playerData.Money -= cost;
            player.PlayerControllerCallback.Success();
        }
        
        private void AcceptOrder(Player player, Order order)
        {
            order = Orders.FirstOrDefault(o => o.Id == order.Id);
            if (order == null)
            {
                player.PlayerControllerCallback.Failure(FailureMessage.OrderNotFound);
                return;
            }

            var playerData = PlayersData[player];

            foreach (var kvp in order.Resources)
            {
                var resource = kvp.Key;
                var amount = kvp.Value;

                if (playerData.Storage.CurrentAmounts[resource] < amount)
                {
                    player.PlayerControllerCallback.Failure(FailureMessage.NotEnoughResources);
                    return;
                }
            }

            Orders.Remove(order);

            foreach (var kvp in order.Resources)
            {
                var resource = kvp.Key;
                var amount = kvp.Value;

                playerData.Storage.Remove(resource, amount);
            }

            playerData.Money += order.Cost;
            player.PlayerControllerCallback.Success();
        }

        private void AcceptTrade(Player player, Trade trade)
        {
            trade = Trades.FirstOrDefault(t => t.Id == trade.Id);
            if (trade == null)
            {
                player.PlayerControllerCallback.Failure(FailureMessage.TradeNotFound);
                return;
            }

            var playerData = PlayersData[player];
            var authorData = PlayersData[trade.Author];

            if (playerData.Money - trade.AskingMoney < 0)
            {
                player.PlayerControllerCallback.Failure(FailureMessage.NotEnoughMoney);
                return;
            }

            foreach (var kvp in trade.AskingResources)
            {
                var resource = kvp.Key;
                var amount = kvp.Value;

                if (playerData.Storage.CurrentAmounts[resource] < amount)
                {
                    player.PlayerControllerCallback.Failure(FailureMessage.NotEnoughResources);
                    return;
                }
            }

            Trades.Remove(trade);

            foreach (var kvp in trade.AskingResources)
            {
                var resource = kvp.Key;
                var amount = kvp.Value;

                playerData.Storage.Remove(resource, amount);
                authorData.Storage.Add(resource, amount);
            }

            foreach (var kvp in trade.GivenResources)
            {
                var resource = kvp.Key;
                var amount = kvp.Value;

                playerData.Storage.Add(resource, amount);
            }

            playerData.Money -= trade.AskingMoney;
            playerData.Money += trade.GivingMoney;

            authorData.Money += trade.AskingMoney;

            trade.Author.PlayerControllerCallback.PlacedTradeAccepted(trade);
            player.PlayerControllerCallback.Success();
        }

        private void PlaceTrade(Player player, Trade trade)
        {
            trade.Author = player;

            var playerData = PlayersData[player];

            if (playerData.Money - trade.GivingMoney < 0)
            {
                player.PlayerControllerCallback.Failure(FailureMessage.NotEnoughMoney);
                return;
            }

            trade.GivenResources = trade.GivenResources
                .Where(kvp => kvp.Value > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            trade.AskingResources = trade.AskingResources
                .Where(kvp => kvp.Value > 0)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (var kvp in trade.GivenResources)
            {
                var resource = kvp.Key;
                var amount = kvp.Value;

                if (playerData.Storage.CurrentAmounts[resource] < amount)
                {
                    player.PlayerControllerCallback.Failure(FailureMessage.NotEnoughResources);
                    return;
                }
            }

            foreach (var kvp in trade.GivenResources)
            {
                var resource = kvp.Key;
                var amount = kvp.Value;

                playerData.Storage.Remove(resource, amount);
            }

            playerData.Money -= trade.GivingMoney;
            Trades.Add(trade);
            player.PlayerControllerCallback.Success();
        }
    }
}