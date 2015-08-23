namespace FunctionalLiving.Parser

[<AutoOpen>]
module BitHelpers =

    type Bit =
        | One
        | Zero
        override this.ToString() =
            match this with
            | One -> "1"
            | Zero -> "0"


    let bitMasks =
        Seq.unfold (fun bitIndex -> Some((byte(pown 2 bitIndex), bitIndex), bitIndex + 1)) 0
        |> Seq.take 8
        |> Seq.toList
        |> List.rev

    let byteToBitArray b =
        let determineBit (bitMask, bitPosition) =
            if (b &&& bitMask) >>> bitPosition = byte(0) then Zero
            else One

        List.map (determineBit) bitMasks

    let bytesToBits (bytes: byte[]) =
        bytes
        |> Array.toList
        |> List.map byteToBitArray
        |> List.collect id
        |> List.toArray

    let bitsToUInt (bits: Bit[]) =
        let positions = Array.zip bits (Array.rev [|0..Array.length bits - 1|])

        Array.fold (fun acc (bit, index) ->
            match bit with
            | Zero -> acc
            | One -> acc + (pown 2 index)) 0 positions

    let lastBits (byte: byte) amount =
        let i = int(byte)
        (i >>> 0) &&& ((1 <<< amount) - 1)

    let firstBits (byte: byte) amount =
        let i = int(byte)
        let remain = 8 - amount
        (i >>> remain) &&& ((1 <<< amount) - 1)