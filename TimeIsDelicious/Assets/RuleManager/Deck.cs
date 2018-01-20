using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

namespace RuleManager
{
    public interface ICard
    {
        void Reset();
        IObservable<Unit> OnDiscardAsObservable { get; }
    }

    public class Deck<T>
        where T : ICard
    {
        public enum Mode
        {
            OnlyOnce,
            AutoBack
        }

        List<T> deck = new List<T>();
        List<T> discardDeck = new List<T>();
        public readonly int MaxDeck;
        public readonly Mode mode;

        public int Remain
        {
            get
            {
                return deck.Count();
            }
        }  
        public bool Empty
        {
            get
            {
                return Remain == 0;
            }
        }

        public Deck(IEnumerable<T> Source, Mode mode = Mode.OnlyOnce)
        {
            deck.AddRange(Source);
            deck.Shuffle();
            MaxDeck = deck.Count;
            this.mode = mode;
        }

        public IEnumerable<T> OpenTop(int NumberOfSheet)
        {
            if (NumberOfSheet < 0 || NumberOfSheet > MaxDeck)
            {
                MakiMaki.Logger.Error("Exeed max number of orginal deck.");
                return null;
            }

            if (mode == Mode.AutoBack && deck.Count < NumberOfSheet)
            {
                discardDeck.Shuffle();
                deck.AddRange(discardDeck.Select(card =>
                {
                    card.Reset();
                    return card;
                }).ToList());
                discardDeck.Clear();
            }

            int numOpen = Mathf.Min(NumberOfSheet, deck.Count);
            var chunk = deck.Chunks(numOpen);
            deck = deck.Skip(numOpen).ToList();

            return chunk.Select(card =>
            {
                card.OnDiscardAsObservable
                    .First()
                    .Subscribe(_ => discardDeck.Add(card));
                return card;
            }).ToList();
        }
    }
}