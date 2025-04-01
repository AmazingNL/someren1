using someren_application.Models;

namespace someren_application.IRepositories
{
    public interface IDrinksRepository
    {
        List<Drinks> GetAllDrink();
        Drinks? GetById(int drinkId);
        void Add(Drinks drink);
        void Update(Drinks drink);
        void Delete(Drinks drink);
        List<Drinks> Filter(string isAlcoholic);
    }
}
