namespace BP.ColourChimp.Classes
{
    /// <summary>
    /// Enumeration of channel dominance modes.
    /// </summary>
    public enum ChannelDominanceModes
    {
        /// <summary>
        /// No dominant R, G or B channel.
        /// </summary>
        NonDominantRGB = 0,
        /// <summary>
        /// A dominant R, G or B channel.
        /// </summary>
        DominantRGB,
        /// <summary>
        /// No dominant C, M or Y channel.
        /// </summary>
        NonDominantCMY,
        /// <summary>
        /// A dominant R, G or B channel.
        /// </summary>
        DominantCMY,
        /// <summary>
        /// R, G and B channels have equal values.
        /// </summary>
        Gray,
        /// <summary>
        /// R, G and B have no equal values.
        /// </summary>
        NonGray
    }
}