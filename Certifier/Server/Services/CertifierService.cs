using System.Linq;
using System.Threading.Tasks;
using Certify;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;

namespace Server
{
    public class CertifierService : Certifier.CertifierBase
    {
        [Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
        public override Task<CertificateInfoResponse> GetCertificateInfo(Empty request, ServerCallContext context)
        {
            // ClientCertificateMode in Kestrel must be configured to allow client certificates
            // https://docs.microsoft.com/dotnet/api/microsoft.aspnetcore.server.kestrel.https.httpsconnectionadapteroptions.clientcertificatemode

            // Use the following code to get the client certificate as an X509Certificate2 instance:
            //
            var httpContext = context.GetHttpContext();
            var clientCertificate = httpContext.Connection.ClientCertificate;

            var name = string.Join(',', context.AuthContext.PeerIdentity.Select(i => i.Value));
            System.Console.WriteLine("****************");
            System.Console.WriteLine(name);
            var certificateInfo = new CertificateInfoResponse
            {
                HasCertificate = context.AuthContext.IsPeerAuthenticated,
                Name = name
            };

            return Task.FromResult(certificateInfo);
        }
    }
}
