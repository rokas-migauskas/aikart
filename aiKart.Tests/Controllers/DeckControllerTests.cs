using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using aiKart.Controllers;
using aiKart.Interfaces;
using aiKart.Models;
using aiKart.Dtos.DeckDtos;
using aiKart.Dtos.CardDtos;
using aiKart.States;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace aiKart.Tests
{
    public class DeckControllerTests
    {
        private readonly Mock<IDeckService> mockDeckService;
        private readonly Mock<IMapper> mockMapper;
        private readonly DeckController deckController;

        private readonly Mock<IUserDeckService> mockUserDeckService;

        public DeckControllerTests()
        {
            mockDeckService = new Mock<IDeckService>();
            mockMapper = new Mock<IMapper>();
            mockUserDeckService = new Mock<IUserDeckService>();
            deckController = new DeckController(mockDeckService.Object, mockUserDeckService.Object, mockMapper.Object);
        }

        [Fact]
        public void GetAllDecks_ReturnsOk()
        {
            var decks = new List<Deck>();
            var deckDtos = new List<DeckDto> { new DeckDto(1, "Name", 1, null, null, true, true, DateTime.Now, DateTime.Now, null) };

            mockDeckService.Setup(s => s.GetAllDecksIncludingCards()).Returns(decks);
            mockMapper.Setup(m => m.Map<List<DeckDto>>(decks)).Returns(deckDtos);

            var result = deckController.GetAllDecks();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(deckDtos, okResult.Value);
        }


        [Fact]
        public async Task GetDeck_ReturnsNotFound()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(false);

            var result = await deckController.GetDeck(1);

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task GetDeck_ReturnsOk()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(true);
            mockDeckService.Setup(s => s.GetDeckByIdAsync(It.IsAny<int>())).ReturnsAsync(new Deck());
            mockMapper.Setup(m => m.Map<DeckDto>(It.IsAny<Deck>()))
                .Returns(new DeckDto(1, "Name", 1, null, null, true, true, DateTime.Now, DateTime.Now, null));

            var result = await deckController.GetDeck(1);

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void GetCardsInDeck_ReturnsNotFound()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(false);
            var result = deckController.GetCardsInDeck(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetCardsInDeck_ReturnsOk()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(true);
            mockDeckService.Setup(s => s.GetCardsInDeck(It.IsAny<int>())).Returns(new List<Card>());
            mockMapper.Setup(m => m.Map<IEnumerable<CardDto>>(It.IsAny<IEnumerable<Card>>()))
                .Returns(new List<CardDto> { new CardDto(1, 1, "Question", "Answer", "Answered", 1, null) });
            var result = deckController.GetCardsInDeck(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void AddDeck_ReturnsCreatedResponse()
        {
            var addDeckDto = new AddDeckDto("Name", null, 1, null, true);
            var deck = new Deck { Id = 1, Name = "Name" }; // The deck entity that would be "added" by the service.
            mockDeckService.Setup(s => s.DeckExistsByName(It.IsAny<string>())).Returns(false);
            mockDeckService.Setup(s => s.AddDeck(It.IsAny<Deck>())).Returns(true);
            mockUserDeckService.Setup(s => s.AddUserDeck(It.IsAny<UserDeck>())).Returns(true); // Assume that adding a UserDeck relationship is successful.
            mockMapper.Setup(m => m.Map<Deck>(It.IsAny<AddDeckDto>())).Returns(deck);
            mockMapper.Setup(m => m.Map<DeckDto>(It.IsAny<Deck>())).Returns(new DeckDto(deck.Id, deck.Name, 1, null, null, true, false, DateTime.Now, DateTime.Now, null));

            var result = deckController.AddDeck(addDeckDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var deckDtoResult = Assert.IsType<DeckDto>(createdAtActionResult.Value); // Make sure the returned value is DeckDto
            Assert.Equal(deck.Id, deckDtoResult.Id);
        }


        [Fact]
        public void UpdateDeck_ReturnsNotFound()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(false);
            var result = deckController.UpdateDeck(1, new UpdateDeckDto("Name", null, null, true, null));
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateDeck_ReturnsNoContent()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(true);
            mockDeckService.Setup(s => s.GetDeckById(It.IsAny<int>())).Returns(new Deck());
            mockDeckService.Setup(s => s.UpdateDeck(It.IsAny<Deck>())).Returns(true);
            var result = deckController.UpdateDeck(1, new UpdateDeckDto("Name", null, null, true, null));
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteDeck_ReturnsNotFound()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(false);
            var result = deckController.DeleteDeck(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteDeck_ReturnsNoContent()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(true);
            mockDeckService.Setup(s => s.GetDeckById(It.IsAny<int>())).Returns(new Deck());
            mockDeckService.Setup(s => s.DeleteDeck(It.IsAny<Deck>())).Returns(true);
            var result = deckController.DeleteDeck(1);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetAllPublicDecks_NoPublicDecks_ReturnsOkWithEmptyList()
        {
            mockDeckService.Setup(s => s.GetAllDecksIncludingCards()).Returns(new List<Deck>());
            var result = deckController.GetAllPublicDecks();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var decks = okResult.Value as List<DeckDto>;
            Assert.Empty(decks ?? new List<DeckDto>()); // Handle null case
        }

        [Fact]
        public void UpdateDeck_UpdateDataNull_ReturnsBadRequest()
        {
            var result = deckController.UpdateDeck(1, null);
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void UpdateDeck_FailureInUpdate_ReturnsServerError()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(true);
            mockDeckService.Setup(s => s.GetDeckById(It.IsAny<int>())).Returns(new Deck());
            mockDeckService.Setup(s => s.UpdateDeck(It.IsAny<Deck>())).Returns(false);

            var result = deckController.UpdateDeck(1, new UpdateDeckDto("Name", null, null, true, null));
            Assert.IsType<ObjectResult>(result);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void DeleteDeck_FailureInDelete_ReturnsServerError()
        {
            mockDeckService.Setup(s => s.DeckExistsById(It.IsAny<int>())).Returns(true);
            mockDeckService.Setup(s => s.GetDeckById(It.IsAny<int>())).Returns(new Deck());
            mockDeckService.Setup(s => s.DeleteDeck(It.IsAny<Deck>())).Returns(false);

            var result = deckController.DeleteDeck(1);
            Assert.IsType<ObjectResult>(result);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task ClonePublicDeck_SourceDeckDoesNotExist_ReturnsBadRequest()
        {
            mockDeckService.Setup(s => s.ClonePublicDeck(It.IsAny<int>(), It.IsAny<int>())).Throws(new Exception("Deck not found."));
            var result = await deckController.ClonePublicDeck(1, new CloneDeckDto(1));
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

}
