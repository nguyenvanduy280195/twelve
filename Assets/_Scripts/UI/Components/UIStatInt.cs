public class UIStatInt : UIStat<int>
{
    protected override string ToContent(int value) => value.ToString();

    protected override int ToType(string value) => int.Parse(value);
}
