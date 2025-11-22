using Epixx.Models;
using Microsoft.AspNetCore.Components.Routing;
using System.Reflection.Metadata.Ecma335;

namespace Epixx.Services
{
    public class InboundShipmentService : BackgroundService
    {
        private Random _rnd = new Random();
        private List<Pallet> pallets = new List<Pallet>();
        public List<Pallet> GetPallets()
        {
            return pallets; 
        }
        public void RemovePallet(Pallet pallet)
        {
            pallets.Remove(pallet);
        }
        private void CreateNewPallets()
        {
            int random = _rnd.Next(1, 6);
            Pallet pallet = new Pallet();
            switch (random)
            {
                case 1:
                    
                    pallet.Description = "Arbetsstrumpor 3 par";
                    pallet.Height = 130;
                    pallet.Amount = 250;
                    pallet.Location = "Awaiting Storage";
                    break;
                case 2:

                    pallet.Description = "Badrumsvärmare 2000 W 230 V";
                    pallet.Height = 210;
                    pallet.Amount = 5;
                    pallet.Location = "Awaiting Storage";
                    break;
                case 3:

                    pallet.Description = "Domkraft med aluminiumchassi 1,5 t 90-360 mm";
                    pallet.Height = 150;
                    pallet.Amount = 4;
                    pallet.Location = "Awaiting Storage";
                    break;
                case 4:

                    pallet.Description = "Batteridriven mutterdragare 900 Nm 18 V";
                    pallet.Height = 210;
                    pallet.Amount = 30;
                    pallet.Location = "Awaiting Storage";
                    break;
                case 5:

                    pallet.Description = "Kompressor 24 l 1000 W";
                    pallet.Height = 150;
                    pallet.Amount = 4;
                    pallet.Location = "Awaiting Storage";
                    break;
            }
            int noofpallets = _rnd.Next(1, 6);
            //Adding X amount of the same pallet
            for(int i = 0; i < noofpallets; i++)
            {
                Pallet p = new Pallet
                {
                    PalletID = _rnd.Next(10000000, 999999999),
                    Barcode = _rnd.Next(10000000, 999999999),
                    Description = pallet.Description,
                    Height = pallet.Height,
                    Amount = pallet.Amount,
                    Location = pallet.Location,
                };
                pallets.Add(p);
            }
            //Console.WriteLine("\nAdded " + noofpallets + " pallets with status " + pallet.Location + " with a description of " + pallet.Description);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            // This loop runs in the background
            while (!stoppingToken.IsCancellationRequested)
            {
                if (pallets.Count < 50)
                {
                    CreateNewPallets();
                }
                    
                await Task.Delay(1000, stoppingToken); // wait 3 seconds
            }
        }
    }
}
