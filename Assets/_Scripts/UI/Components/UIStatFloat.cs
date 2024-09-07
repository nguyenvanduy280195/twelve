public class UIStatFloat : UIStat<float>
{
    protected override string ToContent(float value) => string.Format("{0:#0.0}", value);

    protected override float ToType(string value) => float.Parse(value);
}
