static class CalculateAttributes
{
    public static int CaculateDamage(Stat.AttackStat attackStat,float fMotionMul, Stat.Resister resister)
    {
        int nPhysics = attackStat.physicalDamage;
        nPhysics = (int)(nPhysics * fMotionMul);

        int nPhysiscRes = 0;
        switch (attackStat.physicsType)
        {
            case Stat.PhysicalType.Slash:
                nPhysiscRes = resister.slash;
                break;
            case Stat.PhysicalType.Strike:
                nPhysiscRes = resister.strike;
                break;
        }

        #region Element
        int nElement = attackStat.element != Stat.Element.None ? attackStat.elementDamage : 0;
        nElement = (int)(nElement * fMotionMul);

        int nElementRes = 0;
        if(attackStat.element!=Stat.Element.None)
            switch (attackStat.element)
            {
                case Stat.Element.Fire:
                    nElementRes = resister.fire;
                    break;
                case Stat.Element.Ice:
                    nElementRes = resister.ice;
                    break;
                case Stat.Element.Thunder:
                    nElementRes = resister.thunder;
                    break;
            }

        int nSubElement = attackStat.subElement != Stat.Element.None ? attackStat.elementDamage : 0;
        nSubElement = (int)(nSubElement * fMotionMul);
        int nSubEleRes = 0;
        if (attackStat.subElement != Stat.Element.None)
            switch (attackStat.element)
            {
                case Stat.Element.Fire:
                    nSubEleRes = resister.fire;
                    break;
                case Stat.Element.Ice:
                    nSubEleRes = resister.ice;
                    break;
                case Stat.Element.Thunder:
                    nSubEleRes = resister.thunder;
                    break;
            }
        #endregion

        #region TotalCalulate
        nPhysics -= nPhysiscRes;
        if (nPhysics <= 0)
            nPhysics = 5; 
        nElement -= nElementRes;
        if (nElement <= 0)
            nElement = 0;
        nSubElement -= nSubEleRes;
        if (nSubElement <= 0)
            nSubElement = 0;

        #endregion

        return nPhysics + nElement;
    }
}
