using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using temis.Api.Controllers.Models.Requests;
using temis.Api.Models.DTO.MemberDto;
using temis.Api.Models.DTO.ViewModel;
using temis.Core.Models;
using temis.Core.Services.Interfaces;

namespace temis.Api.v1.Controllers
{
    /// <summary>
    /// MemberController
    /// </summary>
    [ApiController]
    [Route("api/v1/member")]
    [ExcludeFromCodeCoverage]

    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="mapper"></param>
        public MemberController(IMemberService service, IMapper mapper)
        {
           _memberService = service;
           _mapper = mapper;
        }

        public MemberController(IMemberService service)
        {
           _memberService = service;
        }

        /// <summary>
        /// Get member by id number
        /// </summary>
        /// <param name="idMember"></param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        /// <response code="400">Business logic error, see return message for more info</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        /// <response code="500">Due to server problems, it`s not possible to get your data now</response>

        [HttpGet("{id}")]
        [Authorize(Roles = "coder, advogado")]
        public IActionResult Get([FromRoute] long id)
        {
            var userEntity = _memberService.FindById(id);
            return Ok(_mapper.Map<MemberViewModel>(userEntity));
        }

        /// <summary>
        /// Get all member
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        /// <response code="400">Business logic error, see return message for more info</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        /// <response code="500">Due to server problems, it`s not possible to get your data now</response>

        [HttpGet]
        //[Authorize(Roles = "coder, advogado")]
        public IActionResult Get(int? page, int? limit, string name = "")
        {   
            PageRequest pageRequest = PageRequest.Of(page, limit);
            PageResponse<Member> pageResponse = _memberService.Filter(name, pageRequest);

            if (pageResponse.Content != null || pageResponse.Content.Count != 0)
            {
                return Ok(pageResponse.Content);
            } 

            return Ok(_memberService.FindAll());
        }

        /// <summary>
        /// Cria um usuário
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        /// <response code="400">Business logic error, see return message for more info</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        /// <response code="500">Due to server problems, it`s not possible to get your data now</response>

    /*    [HttpPost]
        public IActionResult Post([FromBody] MemberDto member)
        {
            var userEntity = _memberService.CreateMember(_mapper.Map<Member>(member));
            return Ok(_mapper.Map<MemberViewModel>(userEntity));
        } */

        [HttpPost]
        public ActionResult<Member> Post([FromBody] Member member)
        {
            var userEntity = _memberService.CreateMember(member);
            return Ok(userEntity);
        } 

        /// <summary>
        /// Altera um usuário cadastrado
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        /// <response code="400">Business logic error, see return message for more info</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        /// <response code="500">Due to server problems, it`s not possible to get your data now</response>

        [HttpPut]
        [Authorize(Roles = "")]
        public IActionResult  Put([FromBody] MemberDto member)
        {
            
            var userEntity = _memberService.EditMember(_mapper.Map<Member>(member));
            return Ok(_mapper.Map<MemberViewModel>(userEntity));
        }

        /// <summary>
        /// Altera parcialmente o usuário
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        /// <response code="400">Business logic error, see return message for more info</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        /// <response code="500">Due to server problems, it`s not possible to get your data now</response>

        [HttpPatch("edit")]
        [Authorize(Roles = "")]
        public IActionResult Patch([FromBody]EditPasswordRequest request)
        {
            _memberService.EditPassword(request.Id, request.Password);
            return Ok();
        }

        /// <summary>
        /// Judgment delete.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Delete /api/Member/{id}
        ///     
        /// </remarks>  
        /// <response code="200">Success</response>
        /// <response code="204">No Content</response>
        /// <response code="400">Business logic error, see return message for more info</response>
        /// <response code="401">Unauthorized. Token not present, invalid or expired</response>
        /// <response code="500">Due to server problems, it`s not possible to get your data now</response>

        [HttpDelete("{id}")]
        [Authorize(Roles = "")]
        public ActionResult Delete([FromRoute] long id)
        {
            bool memberNotFound = _memberService.FindById(id) == null;

            if(memberNotFound)
            {
                return NotFound("Member not found");
            }

            _memberService.Delete(id);
            return NoContent();
        }

    }
}
