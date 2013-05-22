﻿using System.Collections.ObjectModel;

namespace SIL.Cog.ViewModels
{
	public class TaskAreaCommandsViewModel : CogViewModelBase
	{
		private readonly ReadOnlyCollection<CommandViewModel> _commands;

		public TaskAreaCommandsViewModel(string displayName, params CommandViewModel[] commands)
			: base(displayName)
		{
			_commands = new ReadOnlyCollection<CommandViewModel>(commands);
		}

		public ReadOnlyCollection<CommandViewModel> Commands
		{
			get { return _commands; }
		}
	}
}