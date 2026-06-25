using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Social_Network.Core.Models.DTOs;

namespace Social_Network.ViewModels
{
    // Обёртка над сообщением со своей командой удаления —
    // нужна, чтобы привязка работала без RelativeSource (на поведениях он недоступен)
    public partial class MessageItemViewModel : ObservableObject
    {
        public MessageDto Dto { get; }
        private readonly Func<MessageDto, Task> _delete;

        public MessageItemViewModel(MessageDto dto, Func<MessageDto, Task> delete)
        {
            Dto = dto;
            _delete = delete;
        }

        public int Id => Dto.Id;
        public bool IsMine => Dto.IsMine;
        public string Text => Dto.Text;
        public DateTime SentAt => Dto.SentAt;

        [RelayCommand]
        private Task Delete() => _delete(Dto);
    }
}
