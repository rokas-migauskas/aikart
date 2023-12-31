using aiKart.Dtos;
using aiKart.Models;

namespace aiKart.Interfaces;

public interface ICardRepository
{
    Card? GetCardById(int id);
    bool CardExists(int id);
    bool AddCardToDeck(Card card);
    bool DeleteCard(Card card);
    bool UpdateCard(Card card);
    bool UpdateCardState(Card card);
    bool Save();
}