using aiKart.Dtos;
using aiKart.Models;

namespace aiKart.Interfaces;

public interface IDeckRepository
{
    ICollection<Deck> GetDecks();
    public IEnumerable<Deck> GetDecksIncludingCards();
    ICollection<Card> GetDeckCards(int deckId);
    Deck? GetDeck(int id);
    Task<Deck> GetDeckByIdAsync(int deckId);
    bool DeckExistsById(int id);
    bool DeckExistsByName(string name);
    bool AddDeck(Deck deck);
    bool DeleteDeck(Deck deck);
    bool UpdateDeck(Deck deck);
    bool Save();
    Task<Deck> CloneDeckAsync(Deck originalDeck);
}