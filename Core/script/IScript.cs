using Core.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.script
{
    public interface IScript
    {
        Entity Entity { get; set; }
        void OnStart();
        void OnUpdate(float deltaTime);
        void OnDestroy();
    }
}
