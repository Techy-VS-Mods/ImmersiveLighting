using System.Collections.Generic;

public static class MaterialRepository
{
    public static List<FuelMaterial> MaterialDatabase()
    {
        return new List<FuelMaterial>
        {
            // HOUSEHOLD
            // Fuels
            new()
            {
                Name = "isopropyl_alcohol", MinTemperature = 980, MaxTemperature = 1200,
                EmissionWavelengths = new[] { 450.0, 480.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "ethanol", MinTemperature = 960, MaxTemperature = 1150,
                EmissionWavelengths = new[] { 435.0, 470.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "methanol", MinTemperature = 920, MaxTemperature = 1120,
                EmissionWavelengths = new[] { 430.0, 460.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "kerosene", MinTemperature = 980, MaxTemperature = 1300,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "lamp_oil", MinTemperature = 900, MaxTemperature = 1250,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "charcoal_briquettes", MinTemperature = 1100, MaxTemperature = 1500,
                EmissionWavelengths = new[] { 600.0, 700.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "natural_gas", MinTemperature = 1200, MaxTemperature = 1400,
                EmissionWavelengths = new[] { 480.0, 490.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "propane", MinTemperature = 980, MaxTemperature = 1300,
                EmissionWavelengths = new[] { 450.0, 470.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "butane", MinTemperature = 960, MaxTemperature = 1250,
                EmissionWavelengths = new[] { 455.0, 480.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "gasoline", MinTemperature = 980, MaxTemperature = 1200,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "diesel_fuel", MinTemperature = 1000, MaxTemperature = 1300,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "lighter_fluid", MinTemperature = 900, MaxTemperature = 1250,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },

            // Solids
            new()
            {
                Name = "paper", MinTemperature = 300, MaxTemperature = 800,
                EmissionWavelengths = new[] { 580.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cardboard", MinTemperature = 450, MaxTemperature = 900,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "newspaper", MinTemperature = 350, MaxTemperature = 750,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cotton_fabric", MinTemperature = 250, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "wool_fabric", MinTemperature = 300, MaxTemperature = 700,
                EmissionWavelengths = new[] { 590.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "polyester_fabric", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "nylon_fabric", MinTemperature = 400, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "upholstery_foam", MinTemperature = 400, MaxTemperature = 800,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.4, 0.6 },
                CleanCombustion = false
            },
            new()
            {
                Name = "sawdust", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "wood_shavings", MinTemperature = 700, MaxTemperature = 950,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Woods
            new()
            {
                Name = "pine_wood", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "oak_wood", MinTemperature = 950, MaxTemperature = 1150,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "birch_wood", MinTemperature = 950, MaxTemperature = 1150,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "firewood", MinTemperature = 900, MaxTemperature = 1200,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "pressed_wood", MinTemperature = 750, MaxTemperature = 950,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "mdf_board", MinTemperature = 750, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "plywood", MinTemperature = 800, MaxTemperature = 1050,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },


            //OUTDOOR
            new()
            {
                Name = "firestarter_cubes", MinTemperature = 900, MaxTemperature = 1200,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "fire_gel", MinTemperature = 950, MaxTemperature = 1150,
                EmissionWavelengths = new[] { 450.0, 480.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "sterno", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 430.0, 460.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "bbq_lighter_fluid", MinTemperature = 950, MaxTemperature = 1250,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "biodiesel", MinTemperature = 950, MaxTemperature = 1250,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "ethanol_fireplace_fuel", MinTemperature = 960, MaxTemperature = 1150,
                EmissionWavelengths = new[] { 435.0, 470.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "camp_stove_fuel", MinTemperature = 1000, MaxTemperature = 1200,
                EmissionWavelengths = new[] { 450.0, 470.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "zippo_lighter_fluid", MinTemperature = 900, MaxTemperature = 1250,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "charcoal_lumps", MinTemperature = 1100, MaxTemperature = 1500,
                EmissionWavelengths = new[] { 600.0, 700.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },

            // Solids
            new()
            {
                Name = "pine_cones", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 580.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "dry_grass", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "dry_leaves", MinTemperature = 300, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "dry_pine_needles", MinTemperature = 350, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "bark_oak", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "bark_birch", MinTemperature = 750, MaxTemperature = 950,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "fatwood", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 580.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "driftwood", MinTemperature = 850, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 580.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "bamboo", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "peat_moss", MinTemperature = 650, MaxTemperature = 850,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "compost_dry", MinTemperature = 600, MaxTemperature = 800,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "straw", MinTemperature = 350, MaxTemperature = 550,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "hay", MinTemperature = 350, MaxTemperature = 550, EmissionWavelengths = new[] { 580.0, 600.0 },
                EmissionWeights = new[] { 0.7, 0.3 }, CleanCombustion = false
            },
            new()
            {
                Name = "mulch", MinTemperature = 700, MaxTemperature = 900,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "corn_husks", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "coconut_husks", MinTemperature = 600, MaxTemperature = 800,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "coconut_shells", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },


            // Building Materials
            new()
            {
                Name = "fiberglass_insulation", MinTemperature = 700, MaxTemperature = 900,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cellulose_insulation", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "asphalt_shingles", MinTemperature = 800, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "tar_paper", MinTemperature = 700, MaxTemperature = 900,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "roofing_felt", MinTemperature = 700, MaxTemperature = 950,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "pvc_pipe", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cpvc_pipe", MinTemperature = 800, MaxTemperature = 1050,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "plastic_wiring_insulation", MinTemperature = 600, MaxTemperature = 800,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "particleboard", MinTemperature = 750, MaxTemperature = 950,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "foam_board_insulation", MinTemperature = 600, MaxTemperature = 850,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "vinyl_flooring", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "linoleum_flooring", MinTemperature = 750, MaxTemperature = 950,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "drywall", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "plaster", MinTemperature = 750, MaxTemperature = 900,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "carpet_synthetic", MinTemperature = 400, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "carpet_padding", MinTemperature = 350, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "rubber_vulcanized", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "silicone_sealant", MinTemperature = 800, MaxTemperature = 1050,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },

            // COOKING / KITCHEN    
            // Cooking Oils and Fats
            new()
            {
                Name = "cooking_oil_vegetable", MinTemperature = 300, MaxTemperature = 450,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cooking_oil_olive", MinTemperature = 350, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cooking_oil_canola", MinTemperature = 350, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "bacon_grease", MinTemperature = 300, MaxTemperature = 450,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "butter", MinTemperature = 250, MaxTemperature = 400,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Sugars and Starches
            new()
            {
                Name = "sugar", MinTemperature = 160, MaxTemperature = 220,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "flour", MinTemperature = 200, MaxTemperature = 350,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cornstarch", MinTemperature = 200, MaxTemperature = 300,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Dry Foods
            new()
            {
                Name = "dry_pasta", MinTemperature = 300, MaxTemperature = 450,
                EmissionWavelengths = new[] { 580.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "dry_rice", MinTemperature = 350, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "popcorn", MinTemperature = 180, MaxTemperature = 250,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "potato_chips", MinTemperature = 200, MaxTemperature = 400,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "crackers", MinTemperature = 200, MaxTemperature = 350,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },

            // Baked and Sweet Foods
            new()
            {
                Name = "bread", MinTemperature = 180, MaxTemperature = 280,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "marshmallows", MinTemperature = 200, MaxTemperature = 300,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "honey", MinTemperature = 150, MaxTemperature = 220,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "chocolate", MinTemperature = 100, MaxTemperature = 160,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },

            // Coffee and Tea
            new()
            {
                Name = "coffee_grounds_dry", MinTemperature = 200, MaxTemperature = 350,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "tea_leaves_dry", MinTemperature = 180, MaxTemperature = 300,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // SYNTHETIC & MANUFACTURED

            // Plastics
            new()
            {
                Name = "polystyrene", MinTemperature = 450, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "acrylic", MinTemperature = 500, MaxTemperature = 650,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "polycarbonate", MinTemperature = 500, MaxTemperature = 650,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "polyethylene", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "polypropylene", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "pvc", MinTemperature = 500, MaxTemperature = 700, EmissionWavelengths = new[] { 590.0, 610.0 },
                EmissionWeights = new[] { 0.6, 0.4 }, CleanCombustion = false
            },
            new()
            {
                Name = "abs_plastic", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "nylon", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "polyester", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "rayon", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "spandex", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },

            // Resins and Rubbers
            new()
            {
                Name = "epoxy_resin", MinTemperature = 600, MaxTemperature = 850,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "fiberglass_resin", MinTemperature = 550, MaxTemperature = 750,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "polyurethane_foam", MinTemperature = 500, MaxTemperature = 700,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "rubber_natural", MinTemperature = 600, MaxTemperature = 900,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "rubber_synthetic", MinTemperature = 700, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },

            // Waxes
            new()
            {
                Name = "paraffin_wax", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "beeswax", MinTemperature = 300, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "candle_wax", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },

            // HOBBY / WORKSHOP
            // Woods and Wood Byproducts
            new()
            {
                Name = "sawdust_hardwood", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "sawdust_softwood", MinTemperature = 750, MaxTemperature = 950,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "wood_scraps", MinTemperature = 850, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "sanding_dust", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Adhesives and Coatings
            new()
            {
                Name = "pva_glue", MinTemperature = 450, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "epoxy_glue", MinTemperature = 500, MaxTemperature = 750,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "paint_thinner", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "lacquer_thinner", MinTemperature = 500, MaxTemperature = 700,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "shellac", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 590.0, 610.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "varnish", MinTemperature = 500, MaxTemperature = 750,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "turpentine", MinTemperature = 450, MaxTemperature = 600,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "linseed_oil", MinTemperature = 300, MaxTemperature = 450,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = false
            },
            new()
            {
                Name = "tung_oil", MinTemperature = 300, MaxTemperature = 450,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = false
            },
            new()
            {
                Name = "wood_stain", MinTemperature = 350, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Wood Chips and Metal Sparks
            new()
            {
                Name = "wood_chips_smoking", MinTemperature = 850, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "metal_filings", MinTemperature = 1000, MaxTemperature = 1500,
                EmissionWavelengths = new[] { 590.0, 620.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "steel_wool", MinTemperature = 1400, MaxTemperature = 1800,
                EmissionWavelengths = new[] { 600.0, 700.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },


            // FIREWORKS AND PYROTECHNICS
            // Pyrotechnic Chemicals
            new()
            {
                Name = "black_powder", MinTemperature = 1200, MaxTemperature = 1800,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "sparklers", MinTemperature = 1300, MaxTemperature = 2000,
                EmissionWavelengths = new[] { 589.0, 700.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "firecracker_fuses", MinTemperature = 1000, MaxTemperature = 1500,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "roman_candles", MinTemperature = 1400, MaxTemperature = 2000,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "fountain_fireworks", MinTemperature = 1400, MaxTemperature = 1900,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "smoke_bombs", MinTemperature = 700, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Metals Used in Fireworks
            new()
            {
                Name = "magnesium_ribbon", MinTemperature = 3000, MaxTemperature = 3300,
                EmissionWavelengths = new[] { 285.0, 450.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "potassium_nitrate", MinTemperature = 600, MaxTemperature = 800,
                EmissionWavelengths = new[] { 404.0, 589.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "sodium_nitrate", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 589.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = true
            },
            new()
            {
                Name = "charcoal_powder", MinTemperature = 1000, MaxTemperature = 1400,
                EmissionWavelengths = new[] { 600.0, 700.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "aluminum_powder", MinTemperature = 2000, MaxTemperature = 2500,
                EmissionWavelengths = new[] { 280.0, 390.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "sulfur_powder", MinTemperature = 600, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Colorants and Enhancers
            new()
            {
                Name = "barium_nitrate", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 515.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = false
            },
            new()
            {
                Name = "copper_sulfate", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 480.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = false
            },
            new()
            {
                Name = "strontium_nitrate", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 620.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = false
            },
            new()
            {
                Name = "zinc_powder", MinTemperature = 1000, MaxTemperature = 1500,
                EmissionWavelengths = new[] { 510.0, 520.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "iron_filings", MinTemperature = 1400, MaxTemperature = 1600,
                EmissionWavelengths = new[] { 590.0, 620.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "steel_filings", MinTemperature = 1500, MaxTemperature = 1700,
                EmissionWavelengths = new[] { 600.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },


            // MISC COMBUSTIBLES
            // Common Household Items
            new()
            {
                Name = "match_heads", MinTemperature = 600, MaxTemperature = 800,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "cigarettes", MinTemperature = 450, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cigar_tobacco", MinTemperature = 450, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "rolling_papers", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Natural Combustibles
            new()
            {
                Name = "hemp_twine", MinTemperature = 450, MaxTemperature = 650,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cotton_balls", MinTemperature = 250, MaxTemperature = 450,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "dryer_lint", MinTemperature = 200, MaxTemperature = 400,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "shoe_polish", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Organic Combustibles
            new()
            {
                Name = "leather_scraps", MinTemperature = 300, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },
            new()
            {
                Name = "human_hair", MinTemperature = 200, MaxTemperature = 450,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "pet_hair", MinTemperature = 200, MaxTemperature = 450,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "bird_feathers", MinTemperature = 250, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },

            // Synthetic Miscellanea
            new()
            {
                Name = "rubber_tires", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "paraffin_blocks", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "wax_tinder", MinTemperature = 350, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },

            // Specialty and Rare Combustibles
            new()
            {
                Name = "road_flares", MinTemperature = 1400, MaxTemperature = 1700,
                EmissionWavelengths = new[] { 620.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = false
            },
            new()
            {
                Name = "glow_sticks", MinTemperature = 300, MaxTemperature = 500,
                EmissionWavelengths = new[] { 520.0, 540.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "emergency_candles", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "hand_sanitizer", MinTemperature = 300, MaxTemperature = 500,
                EmissionWavelengths = new[] { 450.0, 480.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "alcohol_wipes", MinTemperature = 250, MaxTemperature = 450,
                EmissionWavelengths = new[] { 450.0, 480.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "sterno_cans", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 450.0, 480.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "solid_fuel_tablets", MinTemperature = 600, MaxTemperature = 800,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // INDUSTRIAL AND OTHER RARE
            // Coals and Derivatives
            new()
            {
                Name = "coal_anthracite", MinTemperature = 900, MaxTemperature = 1200,
                EmissionWavelengths = new[] { 580.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "coal_bituminous", MinTemperature = 800, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "coke_coal_derived", MinTemperature = 1100, MaxTemperature = 1400,
                EmissionWavelengths = new[] { 600.0, 700.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "peat", MinTemperature = 600, MaxTemperature = 800, EmissionWavelengths = new[] { 580.0, 600.0 },
                EmissionWeights = new[] { 0.6, 0.4 }, CleanCombustion = false
            },

            // Oils and Lubricants
            new()
            {
                Name = "rubber_tires", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "lubricating_oil", MinTemperature = 600, MaxTemperature = 850,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "hydraulic_oil", MinTemperature = 500, MaxTemperature = 800,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cutting_oil", MinTemperature = 500, MaxTemperature = 700,
                EmissionWavelengths = new[] { 589.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "engine_oil", MinTemperature = 600, MaxTemperature = 850,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "grease", MinTemperature = 500, MaxTemperature = 800,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "transmission_fluid", MinTemperature = 500, MaxTemperature = 750,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = false
            },

            // Combustible Metals
            new()
            {
                Name = "magnesium_shavings", MinTemperature = 3100, MaxTemperature = 3300,
                EmissionWavelengths = new[] { 285.0, 450.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "aluminum_filings", MinTemperature = 2000, MaxTemperature = 2500,
                EmissionWavelengths = new[] { 280.0, 390.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "lithium_powder", MinTemperature = 500, MaxTemperature = 800,
                EmissionWavelengths = new[] { 670.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = true
            },
            new()
            {
                Name = "potassium_metal", MinTemperature = 700, MaxTemperature = 850,
                EmissionWavelengths = new[] { 404.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = true
            },
            new()
            {
                Name = "sodium_metal", MinTemperature = 700, MaxTemperature = 900,
                EmissionWavelengths = new[] { 589.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = true
            },

            // Rare Combustibles
            new()
            {
                Name = "barium_nitrate", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 515.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = false
            },
            new()
            {
                Name = "copper_sulfate", MinTemperature = 900, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 480.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = false
            },
            new()
            {
                Name = "strontium_nitrate", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 620.0 }, EmissionWeights = new[] { 1.0 }, CleanCombustion = false
            },
            new()
            {
                Name = "zinc_powder", MinTemperature = 1000, MaxTemperature = 1500,
                EmissionWavelengths = new[] { 510.0, 520.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },
            new()
            {
                Name = "iron_filings", MinTemperature = 1400, MaxTemperature = 1600,
                EmissionWavelengths = new[] { 590.0, 620.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "steel_filings", MinTemperature = 1500, MaxTemperature = 1700,
                EmissionWavelengths = new[] { 600.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = true
            },


            // CULTURAL

            // Ritual and Traditional Woods
            new()
            {
                Name = "sandalwood", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cedarwood", MinTemperature = 850, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.5, 0.5 },
                CleanCombustion = false
            },
            new()
            {
                Name = "frankincense_resin", MinTemperature = 700, MaxTemperature = 900,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "myrrh_resin", MinTemperature = 700, MaxTemperature = 900,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "palo_santo", MinTemperature = 750, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "juniper_branches", MinTemperature = 700, MaxTemperature = 900,
                EmissionWavelengths = new[] { 580.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Incense and Ritual Powders
            new()
            {
                Name = "incense_sticks", MinTemperature = 300, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "sage_bundles", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 589.0, 610.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "copal_resin", MinTemperature = 650, MaxTemperature = 850,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },

            // Natural Oils
            new()
            {
                Name = "ghee", MinTemperature = 300, MaxTemperature = 450, EmissionWavelengths = new[] { 580.0, 600.0 },
                EmissionWeights = new[] { 0.8, 0.2 }, CleanCombustion = true
            },
            new()
            {
                Name = "coconut_oil", MinTemperature = 300, MaxTemperature = 450,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "mustard_oil", MinTemperature = 350, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },

            // Specialty Candles
            new()
            {
                Name = "beeswax_candles", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "tallow_candles", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },
            new()
            {
                Name = "soya_candles", MinTemperature = 400, MaxTemperature = 600,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.8, 0.2 },
                CleanCombustion = true
            },

            // Festival Combustibles
            new()
            {
                Name = "festival_lamps_oil", MinTemperature = 350, MaxTemperature = 500,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.7, 0.3 },
                CleanCombustion = true
            },
            new()
            {
                Name = "wooden_torches", MinTemperature = 850, MaxTemperature = 1100,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "bamboo_torches", MinTemperature = 800, MaxTemperature = 1000,
                EmissionWavelengths = new[] { 580.0, 600.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },

            // Funeral and Memorial Combustibles
            new()
            {
                Name = "funeral_pyres", MinTemperature = 900, MaxTemperature = 1200,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            },
            new()
            {
                Name = "cremation_wood", MinTemperature = 950, MaxTemperature = 1150,
                EmissionWavelengths = new[] { 589.0, 620.0 }, EmissionWeights = new[] { 0.6, 0.4 },
                CleanCombustion = false
            }
        };
    }
}

public class FuelMaterial
{
    public string Name { get; set; }
    public double MinTemperature { get; set; }
    public double MaxTemperature { get; set; }
    public double[] EmissionWavelengths { get; set; }
    public double[] EmissionWeights { get; set; }
    public bool CleanCombustion { get; set; }
}

public class CombustionContext
{
    public bool HighOxygen { get; set; }
    public bool LargeFlame { get; set; }
}