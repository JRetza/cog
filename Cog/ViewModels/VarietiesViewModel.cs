﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using SIL.Cog.Processors;
using SIL.Cog.Services;
using SIL.Collections;
using SIL.Machine;

namespace SIL.Cog.ViewModels
{
	public class VarietiesViewModel : WorkspaceViewModelBase
	{
		private readonly SpanFactory<ShapeNode> _spanFactory; 
		private readonly IDialogService _dialogService;
		private readonly IProgressService _progressService;
		private ListViewModelCollection<ObservableCollection<Variety>, VarietyVarietiesViewModel, Variety> _varieties;
		private VarietyVarietiesViewModel _currentVariety;
		private CogProject _project;
		private bool _isVarietySelected;

		public VarietiesViewModel(SpanFactory<ShapeNode> spanFactory, IDialogService dialogService, IProgressService progressService)
			: base("Varieties")
		{
			_spanFactory = spanFactory;
			_dialogService = dialogService;
			_progressService = progressService;
			TaskAreas.Add(new TaskAreaViewModel("Common tasks", new []
				{
					new CommandViewModel("Add a new variety", new RelayCommand(AddNewVariety)),
					new CommandViewModel("Rename this variety", new RelayCommand(RenameCurrentVariety)), 
					new CommandViewModel("Remove this variety", new RelayCommand(RemoveCurrentVariety))
				}));
			TaskAreas.Add(new TaskAreaViewModel("Other tasks", new []
				{
					new CommandViewModel("Run stemmer on this variety", new RelayCommand(RunStemmer)) 
				}));
		}

		public override void Initialize(CogProject project)
		{
			_project = project;
			if (_varieties != null)
				_varieties.CollectionChanged -= VarietiesChanged;
			Set("Varieties", ref _varieties, new ListViewModelCollection<ObservableCollection<Variety>, VarietyVarietiesViewModel, Variety>(_project.Varieties,
				variety =>
					{
						var vm = new VarietyVarietiesViewModel(_dialogService, _project, variety);
						vm.PropertyChanged += ChildPropertyChanged;
						return vm;
					}));
			_varieties.CollectionChanged += VarietiesChanged;
			CurrentVariety = _varieties.Count > 0 ? _varieties[0] : null;
		}

		private void VarietiesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_currentVariety == null && _varieties.Count > 0)
				CurrentVariety = _varieties[0];
		}

		private void AddNewVariety()
		{
			var vm = new EditVarietyViewModel(_project);
			if (_dialogService.ShowDialog(this, vm) == true)
			{
				var variety = new Variety(vm.Name);
				_project.Varieties.Add(variety);
				IsChanged = true;
				SwitchVariety(variety);
			}
		}

		public override void AcceptChanges()
		{
			base.AcceptChanges();
			ChildrenAcceptChanges(_varieties);
		}

		private void RenameCurrentVariety()
		{
			if (_currentVariety == null)
				return;

			var vm = new EditVarietyViewModel(_project, _currentVariety.ModelVariety);
			if (_dialogService.ShowDialog(this, vm) == true)
			{
				_currentVariety.Name = vm.Name;
				IsChanged = true;
			}
		}

		private void RemoveCurrentVariety()
		{
			if (_currentVariety == null)
				return;

			if (_dialogService.ShowYesNoQuestion(this, "Are you sure you want to remove this variety?", "Cog"))
			{
				int index = _varieties.IndexOf(_currentVariety);
				_project.Varieties.Remove(_currentVariety.ModelVariety);
				if (index == _varieties.Count)
					index--;
				CurrentVariety = _varieties.Count > 0 ? _varieties[index] : null;
				IsChanged = true;
			}
		}

		private void RunStemmer()
		{
			var vm = new RunStemmerViewModel(false);
			if (_dialogService.ShowDialog(this, vm) == true)
			{
				IEnumerable<IProcessor<Variety>> processors = null;
				switch (vm.Method)
				{
					case StemmingMethod.Automatic:
						_currentVariety.ModelVariety.Affixes.Clear();
						processors = new[] {_project.VarietyProcessors["affixIdentifier"], new Stemmer(_spanFactory, _project)};
						break;
					case StemmingMethod.Hybrid:
						processors = new[] {_project.VarietyProcessors["affixIdentifier"], new Stemmer(_spanFactory, _project)};
						break;
					case StemmingMethod.Manual:
						processors = new[] {new Stemmer(_spanFactory, _project)};
						break;
				}
				Debug.Assert(processors != null);
				var pipeline = new Pipeline<Variety>(processors);

				_progressService.ShowProgress(this, () => pipeline.Process(_currentVariety.ModelVariety.ToEnumerable()));
				IsChanged = true;
			}
		}

		public ObservableCollection<VarietyVarietiesViewModel> Varieties
		{
			get { return _varieties; }
		}

		public VarietyVarietiesViewModel CurrentVariety
		{
			get { return _currentVariety; }
			set
			{
				Set(() => CurrentVariety, ref _currentVariety, value);
				IsVarietySelected = _currentVariety != null;
			}
		}

		public bool IsVarietySelected
		{
			get { return _isVarietySelected; }
			set { Set(() => IsVarietySelected, ref _isVarietySelected, value); }
		}

		public override bool SwitchView(Type viewType, object model)
		{
			if (base.SwitchView(viewType, model))
			{
				SwitchVariety((Variety) model);
				return true;
			}
			return false;
		}

		private void SwitchVariety(Variety variety)
		{
			CurrentVariety = _varieties.Single(vm => vm.ModelVariety == variety);
		}
	}
}
