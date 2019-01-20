using System.Threading.Tasks;
using Kuvalda.Core.Checkout;
using Kuvalda.Core.Status;

namespace Kuvalda.Core
{
    public class RepositoryFacade : IRepositoryFacade
    {
        public static string DefaultMessageLabel = "message";
        
        public string MessageLabel { get; set; } = DefaultMessageLabel;
        
        
        private readonly IRepositoryInitializeService _initializeService;
        private readonly ICommitServiceFacade _commitService;
        private readonly IRefsService _refsService;
        private readonly ICheckoutService _checkoutService;
        private readonly IStatusService _statusService;

        public RepositoryFacade(IRepositoryInitializeService initializeService, ICommitServiceFacade commitService,
            IRefsService refsService, ICheckoutService checkoutService, IStatusService statusService)
        {
            _initializeService = initializeService;
            _commitService = commitService;
            _refsService = refsService;
            _checkoutService = checkoutService;
            _statusService = statusService;
        }


        public bool IsInitialized()
        {
            return _initializeService.IsInitialized();
        }

        public async Task Initialize()
        {
            await _initializeService.Initialize();
        }

        public async Task<CommitResult> Commit(CommitOptions options)
        {
            var currentChash = _refsService.GetHeadCommit();
            var commitData = await _commitService.CreateCommit(options.Path, currentChash);
            commitData.Commit.Labels[MessageLabel] = options.Message;
            var chash = await _commitService.StoreCommit(commitData);
            
            return new CommitResult()
            {
                Chash = chash
            };
        }

        public async Task<CheckoutResult> Checkout(CheckoutOptions options)
        {
            var result = await _checkoutService.Checkout(options.CommitHash);
            return new CheckoutResult()
            {
                Added = result.Added,
                Removed = result.Removed,
                Modified = result.Modified
            };
        }

        public async Task<StatusResult> GetStatus()
        {
            var result = await _statusService.GetStatus(_refsService.GetHeadCommit());
            return new StatusResult()
            {
                Added = result.Added,
                Removed = result.Removed,
                Modified = result.Modified
            };
        }
    }
}