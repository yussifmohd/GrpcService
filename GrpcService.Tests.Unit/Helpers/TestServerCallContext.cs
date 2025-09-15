using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcService.Tests.Unit.Helpers
{
    public class TestServerCallContext : ServerCallContext
    {
        private readonly Metadata _requestHeaders;
        private readonly Metadata _responseTrailers;
        private readonly CancellationToken _cancellationToken;
        private readonly AuthContext _authContext;
        private readonly Dictionary<object, object> _userState;
        private WriteOptions? _writeOptions;
        public Metadata? ResponseHeaders { get; private set; }

        private TestServerCallContext(Metadata requestHeader, CancellationToken cancellationToken)
        {
            _requestHeaders = requestHeader;
            _cancellationToken = cancellationToken;
            _responseTrailers = new Metadata();
            _authContext = new AuthContext(string.Empty, []);
            _userState = new Dictionary<object, object>();
        }

        protected override string MethodCore => "MethodName";

        protected override string HostCore => "HostName";

        protected override string PeerCore => "PeerName";

        protected override DateTime DeadlineCore { get; }

        protected override Metadata RequestHeadersCore => _requestHeaders;

        protected override CancellationToken CancellationTokenCore => _cancellationToken;

        protected override Metadata ResponseTrailersCore => _responseTrailers;

        protected override Status StatusCore { get; set; }
        protected override WriteOptions? WriteOptionsCore { get => _writeOptions; set { _writeOptions = value; } }

        protected override AuthContext AuthContextCore => _authContext;

        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options)
        {
            throw new NotImplementedException();
        }

        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders)
        {
            if(responseHeaders != null)
            {
                throw new InvalidOperationException("Response headers have already been written.");
            }

            ResponseHeaders = responseHeaders;
            return Task.CompletedTask;
        }

        protected override IDictionary<object, object> UserStateCore => _userState;

        public static TestServerCallContext Create(Metadata? requestHeaders = null, CancellationToken cancellationToken = default)
        {
            return new TestServerCallContext(requestHeaders ?? new Metadata(), cancellationToken);
        }
    }
}
