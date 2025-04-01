namespace someren_application.Models
{
    public class Drinks : OrderDrinks
    {
        public Drinks(int drinkId, string? drinkName, string? isAlcoholic, int vatRate, int quantity)
        {
            DrinkId = drinkId;
            DrinkName = drinkName;
            IsAlcoholic = isAlcoholic;
            VatRate = vatRate;
            Quantity = quantity;
        }

        public Drinks()
        {
            DrinkId = 0;
            DrinkName = "";
            IsAlcoholic = "";
            VatRate = 0; // Default value
            Quantity = 0; // Default value
        }

        public int DrinkId { get; set; }
        public string? DrinkName { get; set; }
        public string? IsAlcoholic { get; set; }
        public int Quantity { get; set; }
        private int _vatRate; // Private field for VAT rate

        public int VatRate
        {
            get
            {
                return _vatRate;
            }

            set
            {
                _vatRate = value;
                SetVatRate(); // Update VAT rate based on the IsAlcoholic value
            }
        }



        private void SetVatRate()
        {

            if (IsAlcoholic == "Alcoholic")
            {
                _vatRate = 21;
            }
            else if (IsAlcoholic == "Non-Alcoholic")
            {
                _vatRate = 9;
            }

        }
    }
}
