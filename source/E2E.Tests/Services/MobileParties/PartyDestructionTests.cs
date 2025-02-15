using E2E.Tests.Environment;
using E2E.Tests.Util;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Library;
using Xunit.Abstractions;

namespace E2E.Tests.Services.MobileParties;

public class PartyDestructionTests : IDisposable
{
    E2ETestEnvironement TestEnvironement { get; }
    public PartyDestructionTests(ITestOutputHelper output)
    {
        TestEnvironement = new E2ETestEnvironement(output);
    }

    public void Dispose()
    {
        TestEnvironement.Dispose();
    }

    [Fact]
    public void ServerDestroyParty_SyncAllClients()
    {
        // Arrange
        var server = TestEnvironement.Server;

        var partyComponent = GameObjectCreator.CreateInitializedObject<LordPartyComponent>();

        // Act
        string? partyId = null;
        server.Call(() =>
        {
            var party = MobileParty.CreateParty("This should not set", partyComponent, (party) =>
            {
                AccessTools.Method(typeof(LordPartyComponent), "InitializeLordPartyProperties")
                    .Invoke(partyComponent, new object[] { party, Vec2.Zero, 0, null });
            });

            partyId = party.StringId;

            party.RemoveParty();
        });

        // Assert
        Assert.False(server.ObjectManager.TryGetObject<MobileParty>(partyId, out var _));

        foreach (var client in TestEnvironement.Clients)
        {
            Assert.False(client.ObjectManager.TryGetObject<MobileParty>(partyId, out var _));
        }
    }

    [Fact]
    public void ClientDestroyParty_DoesNothing()
    {
        // Arrange
        var server = TestEnvironement.Server;
        var client1 = TestEnvironement.Clients.First();

        var partyComponent = GameObjectCreator.CreateInitializedObject<LordPartyComponent>();

        string? partyId = null;
        server.Call(() =>
        {
            var party = MobileParty.CreateParty("This should not set", partyComponent, (party) =>
            {
                AccessTools.Method(typeof(LordPartyComponent), "InitializeLordPartyProperties")
                    .Invoke(partyComponent, new object[] { party, Vec2.Zero, 0, null });
            });
            partyId = party.StringId;
        });

        Assert.NotNull(partyId);

        // Act
        client1.Call(() =>
        {
            Assert.True(client1.ObjectManager.TryGetObject<MobileParty>(partyId, out var clientParty));

            clientParty.RemoveParty();
        });

        // Assert
        Assert.True(server.ObjectManager.TryGetObject<MobileParty>(partyId, out var _));

        foreach (var client in TestEnvironement.Clients)
        {
            Assert.True(client.ObjectManager.TryGetObject<MobileParty>(partyId, out var _));
        }
    }
}