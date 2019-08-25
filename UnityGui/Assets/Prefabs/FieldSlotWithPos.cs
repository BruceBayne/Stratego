public class FieldSlotWithPos
{

    public  readonly StrategoTypes.FieldSlot Slot;
    public readonly StrategoTypes.FigurePosition Position;

    public readonly int X;
    public readonly int Y;

    public FieldSlotWithPos(StrategoTypes.FieldSlot slot, StrategoTypes.FigurePosition position)
    {
        Slot = slot;
        Position = position;
        X = position.Get.Item1;
        Y = position.Get.Item2;

    }


    public FieldSlotWithPos(StrategoTypes.FieldSlot slot, int x, int y)
    {
        Slot = slot;
        Position = StrategoTypes.FigurePosition.Create(x,y).Value;
        X = x;
        Y = y;

    }

    public override string ToString()
    {
        return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
    }
}