namespace HyberBot.DataPersistence
{
    [Serializable]
    public abstract class Validatable
    {
        public abstract bool Validate();
    }
}
