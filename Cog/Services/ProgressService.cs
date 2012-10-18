﻿using System;
using System.Windows.Input;
using SIL.Cog.ViewModels;
using SIL.Cog.Views;

namespace SIL.Cog.Services
{
	public class ProgressService : IProgressService
	{
		private readonly IDialogService _dialogService;

		public ProgressService(IDialogService dialogService)
		{
			_dialogService = dialogService;
		}

		public bool ShowProgress(object ownerViewModel, ProgressViewModel progressViewModel)
		{
			return _dialogService.ShowDialog(ownerViewModel, progressViewModel) == true;
		}

		public void ShowProgress(object ownerViewModel, Action action)
		{
			using (new OverrideCursor(Cursors.Wait))
				action();
		}
	}
}
