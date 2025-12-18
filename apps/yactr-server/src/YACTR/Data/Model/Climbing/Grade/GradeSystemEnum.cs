namespace YACTR.Data.Model.Climbing.Grade;

public enum GradeSystemEnum
{
    // Bouldering grade systems
    VScale,
    BScale,
    SScale,
    PScale,
    JoshuaTreeScale,
    Fontainebleau,
    AnnotBScale,
    FontTraverse,
    /// Sport and Trad grade systems
    Ewbanks,
    YDS,
    NCCSScale,
    French,
    BritishTechnical,
    UIAA,
    SouthAfrican,
    OldSouthAfrican,
    Saxon,
    Finnish,
    Norwegian,
    Polish,
    BrazilTechnical,
    Swedish,
    Russian,
    // Aid and Ice grade systems
    AidMScale,
    AidAScale,
    AidCScale,
    AlpineIce,
    WaterIce,
    MixedRockIce,
    FerrataSchall,
    FerrataNum,
    FerrataFrench,
    ScottishWinterTechnical
}

public static class GradeSystemEnumExtensions
{
    public static bool IsBoulderingGrade(this GradeSystemEnum grade)
    {
        return new List<GradeSystemEnum>()
        {
            GradeSystemEnum.VScale,
            GradeSystemEnum.BScale,
            GradeSystemEnum.SScale,
            GradeSystemEnum.PScale,
            GradeSystemEnum.JoshuaTreeScale,
            GradeSystemEnum.Fontainebleau,
            GradeSystemEnum.AnnotBScale,
            GradeSystemEnum.FontTraverse,
        }.Contains(grade);
    }

    public static bool IsSportTradGrade(this GradeSystemEnum grade)
    {
        return new List<GradeSystemEnum>()
        {
            GradeSystemEnum.Ewbanks,
            GradeSystemEnum.YDS,
            GradeSystemEnum.NCCSScale,
            GradeSystemEnum.French,
            GradeSystemEnum.BritishTechnical,
            GradeSystemEnum.UIAA,
            GradeSystemEnum.SouthAfrican,
            GradeSystemEnum.OldSouthAfrican,
            GradeSystemEnum.Saxon,
            GradeSystemEnum.Finnish,
            GradeSystemEnum.Norwegian,
            GradeSystemEnum.Polish,
            GradeSystemEnum.BrazilTechnical,
            GradeSystemEnum.Swedish,
            GradeSystemEnum.Russian,
        }.Contains(grade);
    }

    public static bool IsAidIceFerrataGrade(this GradeSystemEnum grade)
    {
        return new List<GradeSystemEnum>()
        {
            GradeSystemEnum.AidMScale,
            GradeSystemEnum.AidAScale,
            GradeSystemEnum.AidCScale,
            GradeSystemEnum.AlpineIce,
            GradeSystemEnum.WaterIce,
            GradeSystemEnum.MixedRockIce,
            GradeSystemEnum.FerrataSchall,
            GradeSystemEnum.FerrataNum,
            GradeSystemEnum.FerrataFrench,
            GradeSystemEnum.ScottishWinterTechnical
        }.Contains(grade);
    }
}