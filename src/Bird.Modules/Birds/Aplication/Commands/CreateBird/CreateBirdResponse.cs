using System;

namespace BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.CreateBird
{
    public class CreateBirdResponse
    {
        public Guid Id { get; set; }

        public CreateBirdResponse(Guid id)
        {
            Id = id;
        }
    }
}
