using System;
using DevTrackR.API.Entities;
using DevTrackR.API.Models;
using DevTrackR.API.Persistence;
using DevTrackR.API.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DevTrackR.API.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageRepository _repository;
        private readonly ISendGridClient _client;
        public PackagesController(IPackageRepository repository, ISendGridClient client)
        {
            _repository = repository;
            _client = client;
        }

        /// <summary>
        /// Listagem de pacotes
        /// </summary>
        /// <returns>Lista de objetos recém-criados</returns >
        /// <response code="201">Cadastro realizado com sucesso</response>
        [HttpGet]
        public IActionResult GetAll(){
            var packages = _repository.GetAll();

            return Ok(packages);
        }

        /// <summary>
        /// Resgatar o pacote pelo código
        /// </summary>
        /// <param name="code">Código do pacote</param>
        /// <returns>Dados do pacote</returns>
        [HttpGet("{code}")]
        public IActionResult GetByCode(string code){
            var package = _repository.GetByCode(code);

            if(package == null)
            {
                return NotFound();
            }

            return Ok(package);
        }

        /// <summary>
        /// Cadastro de um pacote
        /// </summary>
        /// <remarks>
        /// {
        /// "title": "Playstation 5",
        /// "weight": 5,
        /// "senderName": "Temp",
        /// "senderEmail": "winok23862@roxoas.com"
        /// }
        /// </remarks>
        /// <param name="model">Dados do pacote</param>
        /// <returns>Objeto recém-criado do pacote</returns>
        /// <response code="201">Cadastro realizado com sucesso</response>
        /// <response code="400">Dados estão inválidos.</response>
        [HttpPost]
        public async Task<IActionResult> Post(AddPackageInputModel model){
            if(model.Title.Length < 10){
                return BadRequest("Title length must be at least 10 characters long");
            }

            var package = new Package(model.Title, model.Weight);

            _repository.Add(package);

            var message = new SendGridMessage {
                From = new EmailAddress("renanmartins1999@hotmail.com", "renanMX255"),
                Subject = "Your package was dispatched.",
                PlainTextContent = $"Your package with code {package.Code} was dispatched."
            };

            message.AddTo(model.SenderEmail, model.SenderName);

            await _client.SendEmailAsync(message);

            return CreatedAtAction(
                "GetByCode", 
                new {code = package.Code}, 
                package);
        }

        /// <summary>
        /// Atualizar o pacote pelo código
        /// </summary>
        /// <param name="code"></param>
        /// <param name="model"></param>
        /// <returns>Dados do pacote atualizados</returns>
        [HttpPost("{code}/updates")]
        public IActionResult PostUpdate(string code, AddPackageUpdateInputModel model){
            var package = _repository.GetByCode(code);

            if(package == null)
            {
                return NotFound();
            }

            package.AddUpdate(model.Status, model.Delivered);

            _repository.Update(package);

            return NoContent();
        }

        /// <summary>
        /// Deleta um pacote
        /// </summary>
        /// <param name="code"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("{code}/deletes")]
        public IActionResult Delete(string code,  AddPackageUpdateInputModel model){
            return Ok();
        }

    }
}