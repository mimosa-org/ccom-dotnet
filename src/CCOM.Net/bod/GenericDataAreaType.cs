using Oagis;

namespace CommonBOD;

[Serializable]
public class GenericDataAreaType<TVerb, TNoun>
    where TVerb : VerbType, new()
    where TNoun : class, new()
{
    public TVerb Verb { get; init; }
    public TNoun Noun { get; set; }

    public GenericDataAreaType()
    {
        Verb = new TVerb();
        Noun = new TNoun();
    }
}
