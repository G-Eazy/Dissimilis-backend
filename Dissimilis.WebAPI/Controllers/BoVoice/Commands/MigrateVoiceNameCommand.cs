using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dissimilis.WebAPI.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Dissimilis.WebAPI.Controllers.BoVoice
{
    public class MigrateVoiceNameCommand : INotification
    {


        public MigrateVoiceNameCommand()
        {

        }
    }

    public class MigrateVoiceNameCommandHandler : INotificationHandler<MigrateVoiceNameCommand>
    {
        private readonly Repository _repository;

        public MigrateVoiceNameCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public async Task Handle(MigrateVoiceNameCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _repository.context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            var songVoices = await _repository.MigrateSongVoice(cancellationToken);

            foreach( var voice in songVoices)
            {
                voice.VoiceName = voice.Instrument?.Name;
                voice.Instrument = null;
            }

            try
            {
                await _repository.UpdateAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ValidationException("Transaction error, aborting operation. Please try again.");
            }
        }
    }
}