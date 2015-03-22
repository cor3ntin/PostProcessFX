﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ColossalFramework;

namespace PostProcessFX
{
	class SunLensflare
	{
		private Flare m_sunflare;

		public SunLensflare()
		{
			Light[] lights = GameObject.FindObjectsOfType<Light>();

			foreach (Light light in lights)
			{
				if (light.type == LightType.Directional)
				{
					m_sunflare = new Flare();
					
					light.flare = m_sunflare;
				}
			}
		}
	}
}
