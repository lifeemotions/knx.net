namespace FunctionalLiving.Parser

[<AutoOpen>]
module Domain =

    open System

    // Category1_SingleBit
    type SingleBitValue = byte

    type SingleBitState =
    | On
    | Off
     with member this.Text = toString this

    // Category5_Scaling
    type ScalingValue = byte

    // Category7_2ByteUnsignedValue
    type TwoByteUnsignedValue = byte * byte

    // Category9_2ByteFloatValue
    type TwoByteFloatValue = byte * byte

    // Category10_Time
    type TimeValue = byte * byte * byte

    type Day =
    | Monday
    | Tuesday
    | Wednesday
    | Thursday
    | Friday
    | Saturday
    | Sunday
    with member this.Text = toString this

    type Time = Day option * TimeSpan

    // Category11_Date
    type DateValue = byte * byte * byte
