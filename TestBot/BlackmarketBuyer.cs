using S1API.NPCs;
using System;
using UnityEngine;

namespace SilkRoad.Quests
{
    public class BlackmarketBuyer : NPC
    {
        public BlackmarketBuyer() : base("blackmarket_buyer", "Blackmarket", "Buyer") { }


        private static readonly string[] DeliveryAcceptedTexts = {
    "We’ve heard of your product. {amount} of {product}. Impress us.",
    "Your name reached our ears. {amount} units of {product}. Discreet drop. No attention.",
    "Quality like yours doesn't go unnoticed. Deliver {amount} bricks of {product}. Same location.",
    "We're watching, and we’re interested. {amount} {product}. Clean work only.",
    "Consider this a test. {amount} packages of {product}. Don’t disappoint."
};


        private static readonly string[] DeliverySuccessTexts = {
    "The stash was verified. Your reputation just got stronger.",
    "Everything checked out. Not bad for your first real move.",
    "Impressive. Clean drop, tight operation.",
    "The cargo's in place. You're earning our trust.",
    "That’s the kind of work that gets remembered. Good job."
};

        private static readonly string[] RewardDropTexts = {
    "The money's waiting. Don't make us remind you.",
    "Payment's in the usual place. You’ve earned it — for now.",
    "We deliver on our word. Your cut is ready.",
    "Funds are where they should be. Collect, disappear, stay useful.",
    "You've proven your value. Payment is placed. Use it wisely."
};


        internal override void CreateInternal()
        {
            base.CreateInternal();
            Contacts.Buyer = this;
        }

        public void SendDeliveryAccepted(string product, int amount)
        {
            string line = DeliveryAcceptedTexts[UnityEngine.Random.Range(0, DeliveryAcceptedTexts.Length)];
            string formatted = line
                .Replace("{product}", $"<color=#34AD33>{product}</color>")
                .Replace("{amount}", $"<color=#FF0004>{amount}x</color>");

            SendTextMessage(formatted);
        }

        public void SendDeliverySuccess(string product)
        {
            string line = DeliverySuccessTexts[UnityEngine.Random.Range(0, DeliverySuccessTexts.Length)];
            SendTextMessage(line);
        }

        public void SendRewardDropped()
        {
            string line = RewardDropTexts[UnityEngine.Random.Range(0, RewardDropTexts.Length)];
            SendTextMessage(line);
        }
    }
}