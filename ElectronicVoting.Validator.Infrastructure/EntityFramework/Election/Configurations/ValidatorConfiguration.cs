using ElectronicVoting.Validator.Domain.Entities.Election;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Configurations;

public class ElectionValidatorsConfiguration(bool isDevelopmentDocker = false) : IEntityTypeConfiguration<ValidatorNode>
{
    public void Configure(EntityTypeBuilder<ValidatorNode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x=>x.PublicKey).IsRequired(false);
        builder.Property(x=>x.ServerUrl).IsRequired();
        builder.Property(x => x.IsLeader).HasDefaultValue(false).IsRequired();
        
        if (isDevelopmentDocker)
        {
            builder.HasData(
                new ValidatorNode
                {
                    Id = Guid.NewGuid(),
                    IsLeader = true,
                    Name = "electronicvoting.validator_a",
                    ServerUrl = "http://electronicvoting.validator_a:8080",
                    PublicKey = "-----BEGIN CERTIFICATE-----\nMIIEvzCCAqegAwIBAgIUBvJFeyqi4sPnJAzvgYH6WFwXkZQwDQYJKoZIhvcNAQEL\nBQAwgZMxCzAJBgNVBAYTAlBMMRMwEQYDVQQIDApTb21lLVN0YXRlMREwDwYDVQQH\nDAhLYXRvd2ljZTEhMB8GA1UECgwYSW50ZXJuZXQgV2lkZ2l0cyBQdHkgTHRkMRQw\nEgYDVQQDDAtFbGVjdGlvbiBDQTEjMCEGCSqGSIb3DQEJARYUc3p5bWFib3J5c0Bn\nbWFpbC5jb20wHhcNMjUwNzEyMTg0ODIzWhcNMjYwNzAzMTg0ODIzWjBbMQswCQYD\nVQQGEwJQTDETMBEGA1UECAwKU29tZS1TdGF0ZTETMBEGA1UECgwKVmFsaWRhdG9y\nQTENMAsGA1UECwwEVmFsQTETMBEGA1UEAwwKVmFsaWRhdG9yQTCCASIwDQYJKoZI\nhvcNAQEBBQADggEPADCCAQoCggEBAM8ONOUPTHdwl2JqnG4A/JEYN1g/KnVd8P1w\n6zn4ixmXUu3R3NFNJ3+p5XPuq1x5RMgF3GCOfMhGLCjkUdmPuC/oi3Aoxh1droxU\nsQuEfcSowinrSCmSqDXpuEdV9t1W5cLjFSygi4qLIfJKUthe35KnW9nUxG83mthw\nUAbNp4Ph0DunaJKBichIGCV9KbWXhiQvnHjx+44s4DZUUi8etCoa4hwo9NICxdng\nBwjox5xB/f6rE8/gdaJ6mzd1R0LIq99/aOp3fGO8VRxIhZJkFvct5azXEUk6cjdt\nBqFVBhhi0sOGWzaBzIfVLVhCivtoeueholgI4kkudB1ws+q6m/0CAwEAAaNCMEAw\nHQYDVR0OBBYEFOvcJIxNHHiBiG+bpop0IN0Wc8qPMB8GA1UdIwQYMBaAFCUNiXVZ\nObNez+2fe2WoNgOZwLMHMA0GCSqGSIb3DQEBCwUAA4ICAQDKNfNg13PBkdA6kp9O\nXERA156JjuTDEf6FNqUDuNXUL3an82lHNojDUGTNNTElO3N1b822uW46i26YopiU\n95HmuaRbYCi1WWIwfFbi9dP/IgaNDTZe3GLipzD6rNlEVmLtnJdxmON9oEgC3alt\n4jiSZXfG06Gv2R7W85/30ohtjU8WAdQOfJzKsA0udsYZB3GT98HpN2Fsj9+2m722\nXY9xnlCteWU+tMsZGB5fCE7oh+0A6MetmM+NYq0r/y5mbPRP3sVI3SfTpk6RyAQl\nsODniswMHZvxQv17PfpRX2J0sn0pQO6yg9H7WcAVYT4MSxH4JZZUr3BfEvtCRGNB\ns23bBe0bIhrY+N1DgRhqa1eInrAKSkedYD3TVQ/hJr5wCzqGXhsrTZD+CXVedYxn\nMN4BMshgCHH7kOcQwKH69F82cSb4d90y01RcOzT/ddkzAH9XpaCN1y6WQCA+8wRA\n+zVFvak/+0BVdNAbUaIsxDLrsrNKqp80BL1kkIxuudByHs4LZ+XW2lXFTvpnFaHL\nbBH/t2AepP1JAjDcQZE3oWdR3AnjqsuZAJd8zFZ+gTtblKlK3+obnGMrNnO7rpFL\nsRXk/f22F1ioWkaaOJ4YFcsDjZ6Ykx7Tw5/4W2Lx5HgMyO3Zmq4mPqLYU+dBX4F0\npAhYPZG1L15zyh/DbvncsTmQ2w==\n-----END CERTIFICATE-----\n",
                },
                new ValidatorNode
                {
                    Id = Guid.NewGuid(),
                    Name = "electronicvoting.validator_b",
                    ServerUrl = "http://electronicvoting.validator_b:8080",
                    PublicKey = "-----BEGIN CERTIFICATE-----\nMIIEvzCCAqegAwIBAgIUBvJFeyqi4sPnJAzvgYH6WFwXkZQwDQYJKoZIhvcNAQEL\nBQAwgZMxCzAJBgNVBAYTAlBMMRMwEQYDVQQIDApTb21lLVN0YXRlMREwDwYDVQQH\nDAhLYXRvd2ljZTEhMB8GA1UECgwYSW50ZXJuZXQgV2lkZ2l0cyBQdHkgTHRkMRQw\nEgYDVQQDDAtFbGVjdGlvbiBDQTEjMCEGCSqGSIb3DQEJARYUc3p5bWFib3J5c0Bn\nbWFpbC5jb20wHhcNMjUwNzEyMTg0ODIzWhcNMjYwNzAzMTg0ODIzWjBbMQswCQYD\nVQQGEwJQTDETMBEGA1UECAwKU29tZS1TdGF0ZTETMBEGA1UECgwKVmFsaWRhdG9y\nQTENMAsGA1UECwwEVmFsQTETMBEGA1UEAwwKVmFsaWRhdG9yQTCCASIwDQYJKoZI\nhvcNAQEBBQADggEPADCCAQoCggEBAM8ONOUPTHdwl2JqnG4A/JEYN1g/KnVd8P1w\n6zn4ixmXUu3R3NFNJ3+p5XPuq1x5RMgF3GCOfMhGLCjkUdmPuC/oi3Aoxh1droxU\nsQuEfcSowinrSCmSqDXpuEdV9t1W5cLjFSygi4qLIfJKUthe35KnW9nUxG83mthw\nUAbNp4Ph0DunaJKBichIGCV9KbWXhiQvnHjx+44s4DZUUi8etCoa4hwo9NICxdng\nBwjox5xB/f6rE8/gdaJ6mzd1R0LIq99/aOp3fGO8VRxIhZJkFvct5azXEUk6cjdt\nBqFVBhhi0sOGWzaBzIfVLVhCivtoeueholgI4kkudB1ws+q6m/0CAwEAAaNCMEAw\nHQYDVR0OBBYEFOvcJIxNHHiBiG+bpop0IN0Wc8qPMB8GA1UdIwQYMBaAFCUNiXVZ\nObNez+2fe2WoNgOZwLMHMA0GCSqGSIb3DQEBCwUAA4ICAQDKNfNg13PBkdA6kp9O\nXERA156JjuTDEf6FNqUDuNXUL3an82lHNojDUGTNNTElO3N1b822uW46i26YopiU\n95HmuaRbYCi1WWIwfFbi9dP/IgaNDTZe3GLipzD6rNlEVmLtnJdxmON9oEgC3alt\n4jiSZXfG06Gv2R7W85/30ohtjU8WAdQOfJzKsA0udsYZB3GT98HpN2Fsj9+2m722\nXY9xnlCteWU+tMsZGB5fCE7oh+0A6MetmM+NYq0r/y5mbPRP3sVI3SfTpk6RyAQl\nsODniswMHZvxQv17PfpRX2J0sn0pQO6yg9H7WcAVYT4MSxH4JZZUr3BfEvtCRGNB\ns23bBe0bIhrY+N1DgRhqa1eInrAKSkedYD3TVQ/hJr5wCzqGXhsrTZD+CXVedYxn\nMN4BMshgCHH7kOcQwKH69F82cSb4d90y01RcOzT/ddkzAH9XpaCN1y6WQCA+8wRA\n+zVFvak/+0BVdNAbUaIsxDLrsrNKqp80BL1kkIxuudByHs4LZ+XW2lXFTvpnFaHL\nbBH/t2AepP1JAjDcQZE3oWdR3AnjqsuZAJd8zFZ+gTtblKlK3+obnGMrNnO7rpFL\nsRXk/f22F1ioWkaaOJ4YFcsDjZ6Ykx7Tw5/4W2Lx5HgMyO3Zmq4mPqLYU+dBX4F0\npAhYPZG1L15zyh/DbvncsTmQ2w==\n-----END CERTIFICATE-----\n",
                }
            );
        }
    }
}