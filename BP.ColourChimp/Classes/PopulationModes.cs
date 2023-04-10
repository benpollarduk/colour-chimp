namespace BP.ColourChimp.Classes
{
    /// <summary>
    /// Enumeration of population modes.
    /// </summary>
    public enum PopulationModes
    {
        /// <summary>
        /// Red colors are colors where both green and blue channels are minimum.
        /// </summary>
        Reds = 0,
        /// <summary>
        /// Green colors are colors where both red and blue channels are minimum.
        /// </summary>
        Greens,
        /// <summary>
        /// Blue colors are colors where both red and green channels are minimum.
        /// </summary>
        Blues,
        /// <summary>
        /// Cyan colors are colors where both yellow and magenta channels are maxed out.
        /// </summary>
        Cyans,
        /// <summary>
        /// Yellow colors are colors where both cyan and magenta channels are maxed out.
        /// </summary>
        Yellows,
        /// <summary>
        /// Magenta colors are colors where both cyan and yellow channels are maxed out.
        /// </summary>
        Magentas,
        /// <summary>
        /// Grayscale colors are colors that have balanced red, yellow and blue channels.
        /// </summary>
        Grayscale,
        /// <summary>
        /// All presentation framework colors.
        /// </summary>
        PresentationFramework,
        /// <summary>
        /// All system colors.
        /// </summary>
        SystemColors
    }
}