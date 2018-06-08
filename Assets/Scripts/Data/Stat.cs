namespace Stat
{
    [System.Serializable]
    public struct Base
    {
        public int hp;
        public int maxHp;
        public float stamina;
        public float maxStamina;

        public float moveSpeed;
        public float dudgeSpeed;

        public int nItemLoad; // 아이템 사용 관련

        public Resister resister;
    }

    [System.Serializable]
    public struct Resister
    {
        // Phsisc
        public int strike;
        public int slash;

        //Element
        public int fire;
        public int ice;
        public int thunder;
    }

    public enum Element
    {
        Fire,
        Ice,
        Thunder,
        None
    }

    [System.Serializable]
    public enum PhysicalType
    {
        Strike,Slash
    }

    [System.Serializable]
    public struct AttackStat
    {
        public PhysicalType physicsType;
        public int physicalDamage;

        public Element element;
        public int elementDamage;

        public Element subElement;
        public int subDamage;
    }

    [System.Serializable]
    public struct ArmorStat
    {
        public int armorDamage;
        public int armorLimit; // TODO:슈퍼아머 붕괴 혹은 그로기 상태로 변경
        public ArmorType type;
        public bool isSeperation;
    }

    [System.Serializable]
    public enum ArmorType
    {
        Normal,
        SuperArmor, //슈퍼 아머
        Sack //봉지
    }
}

