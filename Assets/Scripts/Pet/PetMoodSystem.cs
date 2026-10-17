using System;
using UnityEngine;

namespace OfflinePet.Pet
{
    public enum PetState
    {
        IdleFloating,
        CuriousLooking,
        HappyRejoicing,
        Eating,
        PlayingWithToy,
        Resting,
        BeingPetted,
        WaitingForCommand
    }

    public class PetMoodSystem
    {
        public float Trust { get; private set; }
        public float Happiness { get; private set; }
        public float Energy { get; private set; }
        public float Hunger { get; private set; }
        public int FriendshipLevel { get; private set; }

        public event Action OnStateChanged;

        public PetMoodSystem(float trust, float happiness, float energy, float hunger, int friendshipLevel)
        {
            Trust = Mathf.Clamp(trust, 0f, 100f);
            Happiness = Mathf.Clamp(happiness, 0f, 100f);
            Energy = Mathf.Clamp(energy, 0f, 100f);
            Hunger = Mathf.Clamp(hunger, 0f, 100f);
            FriendshipLevel = Mathf.Clamp(friendshipLevel, 1, 10);
        }

        public void Feed(float nutrition = 35f)
        {
            Hunger = Mathf.Clamp(Hunger - nutrition, 0f, 100f);
            Happiness = Mathf.Clamp(Happiness + 15f, 0f, 100f);
            Trust = Mathf.Clamp(Trust + 5f, 0f, 100f);
            CheckFriendshipProgress();
            OnStateChanged?.Invoke();
        }

        public void Play(float fun = 25f, float effort = 20f)
        {
            Happiness = Mathf.Clamp(Happiness + fun, 0f, 100f);
            Energy = Mathf.Clamp(Energy - effort, 0f, 100f);
            Hunger = Mathf.Clamp(Hunger + 10f, 0f, 100f);
            Trust = Mathf.Clamp(Trust + 8f, 0f, 100f);
            CheckFriendshipProgress();
            OnStateChanged?.Invoke();
        }

        public void Pet()
        {
            Happiness = Mathf.Clamp(Happiness + 10f, 0f, 100f);
            Trust = Mathf.Clamp(Trust + 4f, 0f, 100f);
            CheckFriendshipProgress();
            OnStateChanged?.Invoke();
        }

        public void Rest(float recovery = 35f)
        {
            Energy = Mathf.Clamp(Energy + recovery, 0f, 100f);
            Happiness = Mathf.Clamp(Happiness + 5f, 0f, 100f);
            OnStateChanged?.Invoke();
        }

        private void CheckFriendshipProgress()
        {
            // Level up when high trust and happiness are attained
            float combinedScore = (Trust * 0.6f) + (Happiness * 0.4f);
            int calculatedLevel = Mathf.Clamp(Mathf.FloorToInt(combinedScore / 20f) + 1, 1, 10);
            if (calculatedLevel > FriendshipLevel)
            {
                FriendshipLevel = calculatedLevel;
            }
        }
    }
}
