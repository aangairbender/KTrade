using KTrade.Core;

namespace KTrade.Client.Common
{
    public static class StringMessage
    {
        public static string From(FailureMessage msg)
        {
            switch (msg)
            {
                case FailureMessage.NotEnoughMoney:
                    return "недостаточно денег";
                case FailureMessage.NotEnoughResources:
                    return "недостаточно ресурсов";
                case FailureMessage.OrderNotFound:
                    return "заказ не найден";
                case FailureMessage.TradeNotFound:
                    return "сделка не найдена";
                default:
                    return "неизвестная ошибка";
            }
        }
    }
}