﻿namespace PostProcessFX.Config
{
    /**
     * The configuration for the Bloom component. 
     */
    public class NewBloomConfig
    {
        // Bloom configuration
        public bool bloomEnabled = true;
        public float softKnee = 0.5f; // Makes transition between under/over-threshold gradual.
        public float radius = 2.0f; // Changes extent of veiling effects in a screen resolution-independent fashion.
        public float threshold = 0.9f; // The bloom threshold.
        public float intensity = 0.7f; // Blend factor of the result image.
        public bool highQuality = false; // Controls filter quality and buffer resolution
        public bool antiFlicker = false; // Reduces flashing noise with an additional filter.
        
        public static NewBloomConfig getDefaultPreset()
        {
            return new NewBloomConfig();
        }

        public static NewBloomConfig getLowPreset()
        {
            NewBloomConfig preset = getDefaultPreset();
            return preset;
        }

        public static NewBloomConfig getMediumPreset()
        {
            NewBloomConfig preset = getLowPreset();
            
            return preset;
        }

        public static NewBloomConfig getHighPreset()
        {
            NewBloomConfig preset = getMediumPreset();
            
            return preset;
        }
    }
}
