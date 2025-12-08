using BlazorGameAPI.Migrations;
using BlazorGameAPI.Services;
using SharedModels.Enum;
using SharedModels.Model;

namespace BlazorGame.Tests.GameTest
{
    public class FullGameFlowTests : TestBase
    {
        [Fact]
        public async Task FullGameFlow_PlayerVsMonster_WithPauseResumeAndScore()
        {
            using var ctx = CreateInMemoryContext();

            var player = new Player
            {
                Username = "hero",
                Email = "hero@blazorgame.com",
                PasswordHash = "1234",
                Level = 1,
                Health = 100,
                MaxHealth = 100,
                Attack = 15,
                Defense = 5,
                Gold = 0,
                ExperiencePoints = 0,
                LevelCap = 100,
                UserType = UserTypeEnum.PLAYER,
                IsActive = true
            };
            ctx.Players.Add(player);

            var room = new Room
            {
                Name = "Chambre de test",
                Description = "Room test",
                DifficultyLevel = DifficultyLevelEnum.EASY,
                ExperienceGained = 50,
                GoldGained = 20,
                IsExplored = false
            };

            var dungeon = new Dungeon
            {
                Name = "DonjonTest",
                Description = "Donjon pour scénario complet",
                Rooms = new List<Room> { room }
            };
            ctx.Dungeons.Add(dungeon);

            var monster = new Monster
            {
                Name = "Slime",
                Level = 1,
                Health = 30,
                Attack = 8,
                Defense = 2
            };
            ctx.Monsters.Add(monster);

            await ctx.SaveChangesAsync();

            var playerService = new PlayerService(ctx);
            var combatService = new CombatService(ctx, playerService);
            var gameSessionService = new GameSessionService(ctx);
            var highScoreService = new HighScoreService(ctx);
            var gameHistoryService = new GameHistoryService(ctx);

            var session = await gameSessionService.StartSession(player.Id, dungeon.IdDungeon);
            Assert.NotNull(session);
            Assert.True(session.IsActive);
            Assert.Equal(player.Id, session.PlayerId);
            Assert.Equal(dungeon.IdDungeon, session.CurrentDungeonId);

            var attackResult = await combatService.PlayerAttacksMonster(player.Id, monster.IdMonster);
            Assert.True(attackResult.Success);
            Assert.True(attackResult.Damage >= 0);

            var monsterAttackResult = await combatService.MonsterAttacksPlayer(player.Id, monster.IdMonster);
            Assert.True(monsterAttackResult.Success);
            Assert.True(monsterAttackResult.Damage >= 0);

            await combatService.ResolveCombatVictory(player.Id, room.Id);

            var updatedPlayer = await ctx.Players.FindAsync(player.Id);
            var updatedRoom = await ctx.Rooms.FindAsync(room.Id);
            Assert.NotNull(updatedPlayer);
            Assert.NotNull(updatedRoom);
            Assert.True(updatedRoom!.IsExplored);
            Assert.True(updatedPlayer!.ExperiencePoints > 0);
            Assert.True(updatedPlayer.Gold > 0);

            var pausedSaved = await gameSessionService.SaveSessionAsync(
                session.SessionId,
                "{\"room\":\"Chambre de test\",\"hp\":80}",
                isPaused: true);
            Assert.True(pausedSaved);

            var pausedSession = await gameSessionService.GetSessionById(session.SessionId);
            Assert.NotNull(pausedSession);
            Assert.True(pausedSession!.IsPaused);
            Assert.Equal("{\"room\":\"Chambre de test\",\"hp\":80}", pausedSession.StateJson);

            var resumedSaved = await gameSessionService.SaveSessionAsync(
                session.SessionId,
                pausedSession.StateJson,
                isPaused: false);
            Assert.True(resumedSaved);

            var resumedSession = await gameSessionService.GetSessionById(session.SessionId);
            Assert.NotNull(resumedSession);
            Assert.False(resumedSession!.IsPaused);

            var history = await gameHistoryService.CreateGameHistory(player.Id);
            Assert.NotNull(history);

            var highScore = await highScoreService.CreateHighScore(player.Id, 1234);
            Assert.NotNull(highScore);
            Assert.Equal(1234, highScore.Score);

            var ended = await gameSessionService.EndSession(session.SessionId);
            Assert.True(ended);

            var endedSession = await gameSessionService.GetSessionById(session.SessionId);
            Assert.NotNull(endedSession);
            Assert.False(endedSession!.IsActive);

            var activeAfterEnd = await gameSessionService.GetActiveSession(player.Id);
            Assert.Null(activeAfterEnd);
        }
    }
}