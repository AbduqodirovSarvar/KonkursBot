using System.Collections.Concurrent;

namespace KonkursBot.Services
{
    public static class StateService
    {
        private static readonly ConcurrentDictionary<long, StateList> KeyValuesState = new();

        public static Task SetUserState(long chatId, StateList state)
        {
            KeyValuesState[chatId] = state;
            return Task.CompletedTask;
        }

        public static Task DeleteState(long Id)
        {
            KeyValuesState.TryRemove(Id, out _);
            return Task.CompletedTask;
        }

        public static StateList? GetUserState(long Id)
        {
            KeyValuesState.TryGetValue(Id, out var state);
            return state;
        }
    }

    public enum StateList
    {
        register_choose_language,
        register_get_fullname,
        register_get_contact
    }
}
