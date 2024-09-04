public class UIStatString : UIStat<string>
{
    protected override string ToContent(string value) => value;
    protected override string ToType(string value) => value;
}

