using System.Threading.Tasks;

namespace MDKControl.Core.ViewModels
{
	public interface IState
	{
        Task SaveState();
        Task UpdateState();
        Task InitState();
	}
}
