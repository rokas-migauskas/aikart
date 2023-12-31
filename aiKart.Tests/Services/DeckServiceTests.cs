using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using aiKart.Data;
using aiKart.Services;
using aiKart.Interfaces;
using aiKart.Models;
using aiKart.Utils;

namespace aiKart.Tests
{
    public class DeckServiceTests
    {
        private readonly Mock<IDeckRepository> _mockDeckRepository;
        private readonly DeckService _deckService;
        private readonly Mock<IUserDeckService> _mockUserDeckService;
        private readonly Mock<ICardRepository> _mockCardRepository;
        
        private readonly Shuffler<Card> _shuffler;

        public DeckServiceTests()
        {
            _mockDeckRepository = new Mock<IDeckRepository>();
            _shuffler = new Shuffler<Card>();
            _mockUserDeckService = new Mock<IUserDeckService>();
            _mockCardRepository = new Mock<ICardRepository>();
            _deckService = new DeckService(_mockDeckRepository.Object, _shuffler, _mockUserDeckService.Object, _mockCardRepository.Object);
            
            
        }

        [Fact]
        public void GetAllDecksIncludingCards_ReturnsAllDecksWithCards()
        {
            var decks = new List<Deck>
            {
                new Deck { Id = 1, Name = "Deck1" },
                new Deck { Id = 2, Name = "Deck2" }
            };
            _mockDeckRepository.Setup(repo => repo.GetDecksIncludingCards()).Returns(decks);

            var result = _deckService.GetAllDecksIncludingCards();
            Assert.Equal(decks, result);
        }

        [Fact]
        public void GetAllDecks_ReturnsAllDecks()
        {
            var decks = new List<Deck>
            {
                new Deck { Id = 1, Name = "Deck1" },
                new Deck { Id = 2, Name = "Deck2" }
            };
            _mockDeckRepository.Setup(repo => repo.GetDecks()).Returns(decks);

            var result = _deckService.GetAllDecks();
            Assert.Equal(decks, result);
        }

        [Fact]
        public void GetDeckById_ReturnsDeck()
        {
            var deck = new Deck { Id = 1, Name = "Deck1" };
            _mockDeckRepository.Setup(repo => repo.GetDeck(1)).Returns(deck);

            var result = _deckService.GetDeckById(1);
            Assert.Equal(deck, result);
        }

        [Fact]
        public void DeckExistsById_ReturnsTrue_WhenDeckExists()
        {
            _mockDeckRepository.Setup(repo => repo.DeckExistsById(1)).Returns(true);
            var result = _deckService.DeckExistsById(1);
            Assert.True(result);
        }

        [Fact]
        public void DeckExistsByName_ReturnsTrue_WhenDeckExists()
        {
            _mockDeckRepository.Setup(repo => repo.DeckExistsByName("Deck1")).Returns(true);
            var result = _deckService.DeckExistsByName("Deck1");
            Assert.True(result);
        }

        [Fact]
        public void AddDeck_ReturnsTrue_WhenAdditionSuccessful()
        {
            var deck = new Deck { Id = 1, Name = "Deck1" };
            _mockDeckRepository.Setup(repo => repo.AddDeck(deck)).Returns(true);
            var result = _deckService.AddDeck(deck);
            Assert.True(result);
        }

        [Fact]
        public void DeleteDeck_ReturnsTrue_WhenDeletionSuccessful()
        {
            var deck = new Deck { Id = 1, Name = "Deck1" };
            _mockDeckRepository.Setup(repo => repo.DeleteDeck(deck)).Returns(true);
            var result = _deckService.DeleteDeck(deck);
            Assert.True(result);
        }

        [Fact]
        public void UpdateDeck_ReturnsTrue_WhenUpdateSuccessful()
        {
            var deck = new Deck { Id = 1, Name = "Deck1" };
            _mockDeckRepository.Setup(repo => repo.UpdateDeck(deck)).Returns(true);
            var result = _deckService.UpdateDeck(deck);
            Assert.True(result);
        }

        [Fact]
        public void GetCardsInDeck_ReturnsCards()
        {
            var cards = new List<Card>
            {
                new Card { Id = 1, Question = "Q1", Answer = "A1" },
                new Card { Id = 2, Question = "Q2", Answer = "A2" }
            };
            _mockDeckRepository.Setup(repo => repo.GetDeckCards(1)).Returns(cards);

            var result = _deckService.GetCardsInDeck(1);
            Assert.Equal(cards, result);
        }

        [Fact]
        public void GetDeckById_ReturnsNull_WhenDeckDoesNotExist()
        {
            _mockDeckRepository.Setup(repo => repo.GetDeck(It.IsAny<int>())).Returns((Deck)null);

            var result = _deckService.GetDeckById(1);
            Assert.Null(result);
        }

        [Fact]
        public void DeckExistsById_ReturnsFalse_WhenDeckDoesNotExist()
        {
            _mockDeckRepository.Setup(repo => repo.DeckExistsById(It.IsAny<int>())).Returns(false);

            var result = _deckService.DeckExistsById(1);
            Assert.False(result);
        }

        [Fact]
        public void DeckExistsByName_ReturnsFalse_WhenDeckDoesNotExist()
        {
            _mockDeckRepository.Setup(repo => repo.DeckExistsByName(It.IsAny<string>())).Returns(false);

            var result = _deckService.DeckExistsByName("NonExistentDeck");
            Assert.False(result);
        }

        [Fact]
        public void AddDeck_ReturnsFalse_WhenAdditionUnsuccessful()
        {
            _mockDeckRepository.Setup(repo => repo.AddDeck(It.IsAny<Deck>())).Returns(false);

            var result = _deckService.AddDeck(new Deck());
            Assert.False(result);
        }

        [Fact]
        public void DeleteDeck_ReturnsFalse_WhenDeckIsNull()
        {
            var result = _deckService.DeleteDeck(null);
            Assert.False(result);
        }

        [Fact]
        public void DeleteDeck_ReturnsFalse_WhenDeletionUnsuccessful()
        {
            _mockDeckRepository.Setup(repo => repo.DeleteDeck(It.IsAny<Deck>())).Returns(false);

            var result = _deckService.DeleteDeck(new Deck());
            Assert.False(result);
        }

        [Fact]
        public void UpdateDeck_ReturnsFalse_WhenUpdateUnsuccessful()
        {
            _mockDeckRepository.Setup(repo => repo.UpdateDeck(It.IsAny<Deck>())).Returns(false);

            var result = _deckService.UpdateDeck(new Deck());
            Assert.False(result);
        }

        [Fact]
        public void GetCardsInDeck_ReturnsEmptyList_WhenNoCardsInDeck()
        {
            _mockDeckRepository.Setup(repo => repo.GetDeckCards(It.IsAny<int>())).Returns(new List<Card>());

            var result = _deckService.GetCardsInDeck(1);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetDeckByIdAsync_ReturnsDeck_WhenDeckExists()
        {
            var deckId = 1;
            var deck = new Deck { Id = deckId, Name = "Test Deck" };
            _mockDeckRepository.Setup(repo => repo.GetDeckByIdAsync(deckId)).ReturnsAsync(deck);

            var result = await _deckService.GetDeckByIdAsync(deckId);

            Assert.Equal(deck, result);
        }

        [Fact]
        public async Task GetDeckByIdAsync_ReturnsNull_WhenDeckDoesNotExist()
        {
            var deckId = 1;
            _mockDeckRepository.Setup(repo => repo.GetDeckByIdAsync(deckId)).ReturnsAsync((Deck)null);

            var result = await _deckService.GetDeckByIdAsync(deckId);

            Assert.Null(result);
        }

        [Fact]
        public void GetShuffledDeckCards_ReturnsShuffledCards()
        {
            var deckId = 1;
            var cards = new List<Card> { new Card(), new Card() };
            _mockDeckRepository.Setup(repo => repo.GetDeckCards(deckId)).Returns(cards);

            var result = _deckService.GetShuffledDeckCards(deckId);

            Assert.Equal(cards.Count, result.Count());
            // Additional checks can be added to ensure shuffling logic is correct
        }

        [Fact]
        public async Task ClonePublicDeck_SuccessfulClone_ReturnsClonedDeck()
        {
            var deckId = 1;
            var userId = 1;
            var originalDeck = new Deck { Id = deckId, Name = "Original Deck", IsPublic = true };
            var clonedDeck = new Deck { Id = 2, Name = "Cloned Deck" };

            _mockDeckRepository.Setup(repo => repo.GetDeckByIdAsync(deckId)).ReturnsAsync(originalDeck);
            _mockDeckRepository.Setup(repo => repo.CloneDeckAsync(originalDeck)).ReturnsAsync(clonedDeck);

            var result = await _deckService.ClonePublicDeck(deckId, userId);

            Assert.Equal(clonedDeck, result);
            // Additional assertions for user deck relationship can be added
        }

        [Fact]
        public void GetUserCardsInDeck_ReturnsFilteredCards()
        {
            var deckId = 1;
            var cards = new List<Card>
        {
            new Card { LastRepetition = DateTime.Now.AddDays(-2), IntervalInDays = 1 },
            new Card { LastRepetition = DateTime.Now, IntervalInDays = 3 }
        };
            _mockDeckRepository.Setup(repo => repo.GetDeckCards(deckId)).Returns(cards);

            var result = _deckService.GetUserCardsInDeck(deckId);

            Assert.Single(result); // Expecting only one card based on the filtering logic
        }
    }
}
