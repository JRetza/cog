﻿using System;
using SIL.Cog.Domain;

namespace SIL.Cog.Application.Services
{
	public interface IProjectService
	{
		event EventHandler<EventArgs> ProjectOpened;

		bool Init();
		bool New(object ownerViewModel);
		bool Open(object ownerViewModel);
		bool Close(object ownerViewModel);
		bool Save(object ownerViewModel);
		bool SaveAs(object ownerViewModel);

		bool IsChanged { get; }
		CogProject Project { get; }
		string ProjectName { get; }
		bool AreAllVarietiesCompared { get; }
	}
}
