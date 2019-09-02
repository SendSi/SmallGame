using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public interface IUIFactoryExtend : IUIFactory
    {
        Task<UI> CreateAsync(Scene scene, string type, GameObject parent);
        void AddSubComponent(UI ui);
        void RemoveSubComponents();
    }
}
