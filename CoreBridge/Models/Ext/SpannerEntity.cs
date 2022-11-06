namespace CoreBridge.Models.Ext
{
    public abstract class SpannerEntity
    { 
        public abstract void SetPrimaryKey();
        public abstract object GetPrimaryKey();

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime CreatedAt { set; get; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime UpdatedAt { set; get; }

        public string GenerateId()
        {
            Guid g = Guid.NewGuid();
            return g.ToString("N");
        }

    }
}
