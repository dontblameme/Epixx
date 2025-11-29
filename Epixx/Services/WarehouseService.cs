using Epixx.Models;

namespace Epixx.Services
{
    public class WarehouseService
    {
        private Random _rnd = new Random();
        private int Spots(int start, int end) => end - start + 1;
        public Row[] Warehouse {  get; set; }
        public WarehouseService() { 
            Row[] rows = CreateWarehouse();
            Warehouse = rows;
        }
        //1.
        private Row[] CreateWarehouse()
        {
            List<RowConfig> spotconfig = GetSpotConfig();
            Row[] rows = new Row[spotconfig.Count];
            int rowsleftwithsameheightconfig = 3 * _rnd.Next(5, 11); //raderna är olika höga på vissa ställen oftast, speciellt i början
            //for each block like RA, RB etc..
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = new Row();
                rows[i].Name = spotconfig[i].Name;
                rows[i].Type = spotconfig[i].Type;
                int noofspotsinthiscol = _rnd.Next(5, 8);
                List<ColConfig> heights = GetNewPalletSpotHeights(noofspotsinthiscol);
                //for each spot in the current iteration of block like RA101 or RA401 etc..
                for (int j = 0; j < spotconfig[i].Spots ; j++)
                {
                  
                    if (rowsleftwithsameheightconfig != 0)
                    {
                        //Now we going verical height adding pallet spots
                        for(int k = 0; k < heights.Count; k++)
                        {
                            string location = spotconfig[i].Name.Contains('1') ? (255 - spotconfig[i].Spots) + j + 1 + " " + heights[k].ColNumber : (555 - spotconfig[i].Spots) + j + 1 + " " + heights[k].ColNumber;
                            string rowname = rows[i].Name.Remove(2, 1);
                            rows[i].PalletSpots.Add(new PalletSpot { Height = heights[k].height, Category = rows[i].Type ,Location = rowname + " " + location});
                        }

                    }
                    else
                    {
                        heights = GetNewPalletSpotHeights(noofspotsinthiscol);
                        rowsleftwithsameheightconfig = 3 * _rnd.Next(5, 11);
                    }
                        rowsleftwithsameheightconfig--;
                }
               
            }
            return rows;
        }
        private List<ColConfig> GetNewPalletSpotHeights(int numberofcols)
        {
            List<ColConfig> colheights = new List<ColConfig>();
            const int maxheight = 250;
            const int mediumheight = 170;
            const int minheight = 110;
            int rand = _rnd.Next(1, 3);
            if (numberofcols == 7)
            {
                colheights.Add(new ColConfig { ColNumber = 1, height = minheight });
                colheights.Add(new ColConfig { ColNumber = 2, height = minheight });
                colheights.Add(new ColConfig { ColNumber = 3, height = minheight });
                colheights.Add(new ColConfig { ColNumber = 4, height = minheight });
                colheights.Add(new ColConfig { ColNumber = 5, height = maxheight });
                colheights.Add(new ColConfig { ColNumber = 6, height = mediumheight + 40});
                colheights.Add(new ColConfig { ColNumber = 7, height = 150 });
                return colheights;
                
            }
            else if (numberofcols == 5)
            {

                switch (rand)
                {
                    case 1:
                        colheights.Add(new ColConfig { ColNumber = 1, height = mediumheight });
                        colheights.Add(new ColConfig { ColNumber = 2, height = maxheight });
                        colheights.Add(new ColConfig { ColNumber = 3, height = maxheight });
                        colheights.Add(new ColConfig { ColNumber = 4, height = 230 });
                        colheights.Add(new ColConfig { ColNumber = 5, height = 150 });
                        break;
                    case 2:
                        colheights.Add(new ColConfig { ColNumber = 1, height = maxheight });
                        colheights.Add(new ColConfig { ColNumber = 2, height = maxheight });
                        colheights.Add(new ColConfig { ColNumber = 3, height = maxheight });
                        colheights.Add(new ColConfig { ColNumber = 4, height = 150 });
                        colheights.Add(new ColConfig { ColNumber = 5, height = 150 });
                        break;
                }
            }
            else if (numberofcols == 6)
            {
                
                switch (rand)
                {
                    case 1:
                        colheights.Add(new ColConfig { ColNumber = 1, height = mediumheight });
                        colheights.Add(new ColConfig { ColNumber = 2, height = maxheight });
                        colheights.Add(new ColConfig { ColNumber = 3, height = minheight });
                        colheights.Add(new ColConfig { ColNumber = 4, height = minheight });
                        colheights.Add(new ColConfig { ColNumber = 5, height = maxheight + 10 });
                        colheights.Add(new ColConfig { ColNumber = 6, height = 150 });
                        break;
                    case 2:
                        colheights.Add(new ColConfig { ColNumber = 1, height = maxheight });
                        colheights.Add(new ColConfig { ColNumber = 2, height = minheight });
                        colheights.Add(new ColConfig { ColNumber = 3, height = maxheight });
                        colheights.Add(new ColConfig { ColNumber = 4, height = mediumheight });
                        colheights.Add(new ColConfig { ColNumber = 5, height = minheight + 10 });
                        colheights.Add(new ColConfig { ColNumber = 6, height = 150 });
                        break;
                }
               
            }
           
            return colheights;

        }

       
        private List<RowConfig> GetSpotConfig()
        {
            return new List<RowConfig>
            {
                // RA → RG
                new RowConfig { Name = "RA1", Type = Category.Electronics, Spots = Spots(185, 255) },
                new RowConfig { Name = "RA2", Type = Category.Electronics, Spots = Spots(485, 555) },
                new RowConfig { Name = "RB1", Type = Category.Electronics, Spots = Spots(185, 255) },
                new RowConfig { Name = "RB2", Type = Category.Electronics, Spots = Spots(485, 555) },
                new RowConfig { Name = "RC1", Type = Category.Electronics, Spots = Spots(185, 255) },
                new RowConfig { Name = "RC2", Type = Category.Electronics, Spots = Spots(485, 555) },
                new RowConfig { Name = "RD1", Type = Category.Furniture, Spots = Spots(179, 255) },
                new RowConfig { Name = "RD2", Type = Category.Furniture, Spots = Spots(479, 555) },
                new RowConfig { Name = "RE1", Type = Category.Furniture, Spots = Spots(179, 255) },
                new RowConfig { Name = "RE2", Type = Category.Furniture, Spots = Spots(479, 555) },
                new RowConfig { Name = "RF1", Type = Category.Furniture, Spots = Spots(162, 255) },
                new RowConfig { Name = "RF2", Type = Category.Furniture, Spots = Spots(455, 555) },
                new RowConfig { Name = "RG1", Type = Category.Clothing, Spots = Spots(155, 255) },
                new RowConfig { Name = "RG2", Type = Category.Clothing, Spots = Spots(401, 555) },

                // RH → RP
                new RowConfig { Name = "RH1", Type = Category.Clothing, Spots = Spots(101, 255) },
                new RowConfig { Name = "RH2", Type = Category.Clothing, Spots = Spots(401, 555) },
                new RowConfig { Name = "RI1", Type = Category.Clothing, Spots = Spots(101, 255) },
                new RowConfig { Name = "RI2", Type = Category.Clothing, Spots = Spots(401, 555) },
                new RowConfig { Name = "RJ1", Spots = Spots(101, 255) },
                new RowConfig { Name = "RJ2", Spots = Spots(401, 555) },
                new RowConfig { Name = "RK1", Spots = Spots(101, 255) },
                new RowConfig { Name = "RK2", Spots = Spots(401, 555) },
                new RowConfig { Name = "RL1", Spots = Spots(101, 255) },
                new RowConfig { Name = "RL2", Spots = Spots(401, 555) },
                new RowConfig { Name = "RM1", Spots = Spots(101, 255) },
                new RowConfig { Name = "RM2", Spots = Spots(401, 555) },
                new RowConfig { Name = "RN1", Spots = Spots(101, 255) },
                new RowConfig { Name = "RN2", Spots = Spots(401, 555) },
                new RowConfig { Name = "RO1", Spots = Spots(101, 255) },
                new RowConfig { Name = "RO2", Spots = Spots(401, 555) },
                new RowConfig { Name = "RP1", Spots = Spots(101, 255) },
                new RowConfig { Name = "RP2", Spots = Spots(401, 555) },
                // RQ → SJ 
                new RowConfig { Name = "RQ1", Spots = Spots(101, 255) },  // left
                new RowConfig { Name = "RQ2", Spots = Spots(416, 555) },  // right
                new RowConfig { Name = "RR1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RR2", Spots = Spots(416, 555) },
                new RowConfig { Name = "RS1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RS2", Spots = Spots(416, 555) },
                new RowConfig { Name = "RT1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RT2", Spots = Spots(416, 555) },
                new RowConfig { Name = "RU1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RU2", Spots = Spots(416, 555) },
                new RowConfig { Name = "RV1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RV2", Spots = Spots(416, 555) },
                new RowConfig { Name = "RW1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RW2", Spots = Spots(416, 555) },
                new RowConfig { Name = "RX1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RX2", Spots = Spots(416, 555) },
                new RowConfig { Name = "RY1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RY2", Spots = Spots(416, 555) },
                new RowConfig { Name = "RZ1", Spots = Spots(116, 255) },
                new RowConfig { Name = "RZ2", Spots = Spots(416, 555) },

                // SA → SJ
                new RowConfig { Name = "SA1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SA2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SB1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SB2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SC1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SC2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SD1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SD2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SE1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SE2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SF1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SF2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SG1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SG2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SH1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SH2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SI1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SI2", Spots = Spots(416, 555) },
                new RowConfig { Name = "SJ1", Spots = Spots(116, 255) },
                new RowConfig { Name = "SJ2", Spots = Spots(416, 555) },
            };
        }

       
    }
}
