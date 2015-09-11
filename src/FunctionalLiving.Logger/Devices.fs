[<AutoOpen>]
module Devices

let devices =
    [
        // 1.001 Switches
        //{ Address = "0/1/0"; Type = DatapointType.Switch; Description = "Verlichting - Leefruimte - Spots TV - Aan/Uit" }
        //{ Address = "0/1/22"; Type = DatapointType.Switch; Description = "Verlichting - Bureau - Centraal - Aan/Uit" }

        // 1.002 Toggles (boolean)
        { Address = "0/5/0"; Type = DatapointType.Toggle; Description = "Beweging in inkomhal" }
        { Address = "0/5/1"; Type = DatapointType.Toggle; Description = "Beweging in toilet beneden" }
        { Address = "0/5/2"; Type = DatapointType.Toggle; Description = "Beweging in berging" }
        { Address = "0/5/3"; Type = DatapointType.Toggle; Description = "Beweging in garage" }
        { Address = "0/5/4"; Type = DatapointType.Toggle; Description = "Beweging in nachthal trap" }
        { Address = "0/5/5"; Type = DatapointType.Toggle; Description = "Beweging in nachthal badkamer" }
        { Address = "0/5/6"; Type = DatapointType.Toggle; Description = "Beweging in badkamer" }
        { Address = "0/5/7"; Type = DatapointType.Toggle; Description = "Beweging in toilet boven" }

        { Address = "1/0/1"; Type = DatapointType.Toggle; Description = "Neerslag buiten" }
        { Address = "1/0/2"; Type = DatapointType.Toggle; Description = "Droog buiten" }

        // 7.007 Time (h)
        { Address = "0/7/2"; Type = DatapointType.Duration; Description = "Aantal uren activiteit handdoekdroger" }
        { Address = "0/7/4"; Type = DatapointType.Duration; Description = "Aantal uren activiteit oven" }
        { Address = "0/7/6"; Type = DatapointType.Duration; Description = "Aantal uren activiteit microgolf" }
        { Address = "0/7/8"; Type = DatapointType.Duration; Description = "Aantal uren activiteit stopcontacten buiten" }
        { Address = "0/7/10"; Type = DatapointType.Duration; Description = "Aantal uren activiteit droogkast" }
        { Address = "0/7/12"; Type = DatapointType.Duration; Description = "Aantal uren activiteit wasmachine" }
        { Address = "0/7/14"; Type = DatapointType.Duration; Description = "Aantal uren activiteit diepvries" }
        { Address = "0/7/16"; Type = DatapointType.Duration; Description = "Aantal uren activiteit koelkast" }
        { Address = "0/7/18"; Type = DatapointType.Duration; Description = "Aantal uren activiteit vaatwas" }
        { Address = "0/7/20"; Type = DatapointType.Duration; Description = "Aantal uren activiteit dampkap" }

        // 7.012 Current (mA)
        { Address = "0/7/3"; Type = DatapointType.Current; Description = "Huidig verbruik handdoekdroger" }
        { Address = "0/7/5"; Type = DatapointType.Current; Description = "Huidig verbruik oven" }
        { Address = "0/7/7"; Type = DatapointType.Current; Description = "Huidig verbruik microgolf" }
        { Address = "0/7/9"; Type = DatapointType.Current; Description = "Huidig verbruik stopcontacten buiten" }
        { Address = "0/7/11"; Type = DatapointType.Current; Description = "Huidig verbruik droogkast" }
        { Address = "0/7/13"; Type = DatapointType.Current; Description = "Huidig verbruik wasmachine" }
        { Address = "0/7/15"; Type = DatapointType.Current; Description = "Huidig verbruik diepvries" }
        { Address = "0/7/17"; Type = DatapointType.Current; Description = "Huidig verbruik koelkast" }
        { Address = "0/7/19"; Type = DatapointType.Current; Description = "Huidig verbruik vaatwas" }
        { Address = "0/7/21"; Type = DatapointType.Current; Description = "Huidig verbruik dampkap" }

        // 9.001 Temperate (degrees C)
        { Address = "0/4/0"; Type = DatapointType.Temperature; Description = "Inkomhal (Plafond)" }
        { Address = "0/4/1"; Type = DatapointType.Temperature; Description = "Toilet beneden (Plafond)" }
        { Address = "0/4/2"; Type = DatapointType.Temperature; Description = "Berging (Plafond)" }
        { Address = "0/4/3"; Type = DatapointType.Temperature; Description = "Garage (Plafond)" }
        { Address = "0/4/4"; Type = DatapointType.Temperature; Description = "Nachthal trap (Plafond)" }
        { Address = "0/4/5"; Type = DatapointType.Temperature; Description = "Nachthal badkamer (Plafond)" }
        { Address = "0/4/6"; Type = DatapointType.Temperature; Description = "Badkamer (Plafond)" }
        { Address = "0/4/7"; Type = DatapointType.Temperature; Description = "Toilet boven (Plafond)" }
        { Address = "0/4/8"; Type = DatapointType.Temperature; Description = "Slaapkamer 1 (Gezamelijk)" }
        { Address = "0/4/9"; Type = DatapointType.Temperature; Description = "Slaapkamer 2 (Merel)" }
        { Address = "0/4/10"; Type = DatapointType.Temperature; Description = "Slaapkamer 3 (Norah)" }
        { Address = "0/4/11"; Type = DatapointType.Temperature; Description = "Bureau" }
        { Address = "0/4/12"; Type = DatapointType.Temperature; Description = "Badkamer" }
        { Address = "0/4/13"; Type = DatapointType.Temperature; Description = "Weerstation" }
        { Address = "0/4/14"; Type = DatapointType.Temperature; Description = "Keuken" }

        { Address = "1/3/0"; Type = DatapointType.Temperature; Description = "Boiler - Buitentemperatuur" }
        { Address = "1/3/1"; Type = DatapointType.Temperature; Description = "Boiler - Temperatuur" }
        { Address = "1/3/5"; Type = DatapointType.Temperature; Description = "Zonnecollector - Zonnecollector temperatuur" }
        { Address = "1/3/6"; Type = DatapointType.Temperature; Description = "Zonnecollector - Zonnecylinder temperatuur" }
        { Address = "1/3/12"; Type = DatapointType.Temperature; Description = "Vloerverwarming - Temperatuur" }
        { Address = "1/3/17"; Type = DatapointType.Temperature; Description = "Warmwater - Temperatuur" }

        // 9.004 Light (lux)
        { Address = "0/3/0"; Type = DatapointType.Light; Description = "Lichtsterkte in inkomhal" }
        { Address = "0/3/1"; Type = DatapointType.Light; Description = "Lichtsterkte in toilet beneden" }
        { Address = "0/3/2"; Type = DatapointType.Light; Description = "Lichtsterkte in berging" }
        { Address = "0/3/3"; Type = DatapointType.Light; Description = "Lichtsterkte in garage" }
        { Address = "0/3/4"; Type = DatapointType.Light; Description = "Lichtsterkte in nachthal trap" }
        { Address = "0/3/5"; Type = DatapointType.Light; Description = "Lichtsterkte in nachthal badkamer" }
        { Address = "0/3/6"; Type = DatapointType.Light; Description = "Lichtsterkte in badkamer" }
        { Address = "0/3/7"; Type = DatapointType.Light; Description = "Lichtsterkte in toilet boven" }
        { Address = "0/3/8"; Type = DatapointType.Light; Description = "Lichtsterkte buiten 1" }
        { Address = "0/3/9"; Type = DatapointType.Light; Description = "Lichtsterkte buiten schemering" }
        { Address = "0/3/10"; Type = DatapointType.Light; Description = "Lichtsterkte buiten 2" }
        { Address = "0/3/11"; Type = DatapointType.Light; Description = "Lichtsterkte buiten 3" }

        // 9.005 Speed (m/s)
        { Address = "1/0/0"; Type = DatapointType.Speed; Description = "Windsnelheid buiten" }
    ]