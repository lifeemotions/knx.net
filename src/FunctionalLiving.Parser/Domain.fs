namespace FunctionalLiving.Parser

[<AutoOpen>]
module Domain =

    open System

    type Switching =
    | On
    | Off

    type SwitchingValue = byte

    type Day =
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday
    | Sunday

    type Time = Day option * TimeSpan

    type TimeValue = byte * byte * byte
