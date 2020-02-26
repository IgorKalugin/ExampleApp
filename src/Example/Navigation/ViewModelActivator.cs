using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Example.Navigation
{
    /// <summary>
    /// Helper class to handle view and view model activations. An instance of this class shouldn't be able to outlive its view and view model
    /// </summary>
    public class ViewModelActivator : ReactiveObject, IViewModelActivator
    {
        private readonly List<Action<CompositeDisposable>> blocks = new List<Action<CompositeDisposable>>();
        private readonly Dictionary<Action<CompositeDisposable>, IDisposable> blockDisposables = new Dictionary<Action<CompositeDisposable>, IDisposable>();

        public ViewModelActivator()
        {
            this.WhenAnyValue(x => x.IsActive)
                .Where(isActive => isActive)
                .Subscribe(_ =>
                {
                    foreach (var block in blocks)
                    {
                        Activate(block);
                    }
                });
            
            this.WhenAnyValue(x => x.IsActive)
                .Where(isActive => !isActive)
                .Subscribe(_ =>
                {
                    foreach (var disposable in blockDisposables.Values)
                    {
                        disposable.Dispose();
                    }
                    blockDisposables.Clear();
                });
        }
        
        [Reactive] public bool IsActive { get; set; }

        public IDisposable WhenActivated(Action<CompositeDisposable> block)
        {
            if (blocks.Contains(block))
            {
                throw new ArgumentException("Activation block is already added");
            }
            
            blocks.Add(block);
            if (IsActive)
            {
                Activate(block);
            }

            return Disposable.Create(() =>
            {
                if (blockDisposables.TryGetValue(block, out var blockDisposable))
                {
                    blockDisposable.Dispose();
                    blockDisposables.Remove(block);
                }
                
                blocks.Remove(block);
            });
        }

        private void Activate(Action<CompositeDisposable> block)
        {
            var blockDisposable = new CompositeDisposable();
            block(blockDisposable);
            blockDisposables.Add(block, blockDisposable);
        }
    }
}