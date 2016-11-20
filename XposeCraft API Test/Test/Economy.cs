﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Prva faza hry.
/// 
/// Cielom je zbierat suroviny pomocou jednotiek pracovnikov
/// a pri dostatocnom pocte surovin vytvarat dalsich pracovnikov na zrychlenie ekonomie.
/// </summary>
namespace XposeCraft_UI_API_Prototype_Test.Test
{
	public class Economy
	{
		public Economy()
		{
		}

		public void EconomyStage()
		{
			Worker worker = GetUnits < Worker >[0];
			worker.SendGather(MaterialHelper.GetNearestMaterialTo(worker));

			RegisterEvent(Events.MineralsChanged, args => {
				if (args<Minerals>() > 50)
				{
					Worker worker = base.CreateUnit<Worker>();
					worker.SendGather();
					TryNubianArmory();
				}
			});
		}
	}
}
