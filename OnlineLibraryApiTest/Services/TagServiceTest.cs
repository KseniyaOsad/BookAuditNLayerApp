using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineLibraryApiTest.Services
{
    [TestClass]
    public class TagServiceTest
    {
        private TagService _tagService;

        private Mock<IUnitOfWork> _mockUnitOfWork = new Mock<IUnitOfWork>();

        private Mock<ITagRepository> _mockTagRepository = new Mock<ITagRepository>();

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork.Setup(x => x.TagRepository).Returns(_mockTagRepository.Object);
        }

        // Task<int> CreateTagAsync(Tag tag)

        [TestMethod]
        public async Task Create_Tag_BadRequest()
        {
            _mockTagRepository.Setup(x => x.CreateTagAsync(It.IsAny<Tag>()));
            _tagService = new TagService(_mockUnitOfWork.Object);
            await Assert.ThrowsExceptionAsync<OLBadRequest>(() => _tagService.CreateTagAsync(new Tag()));
            _mockUnitOfWork.Verify(x => x.TagRepository.CreateTagAsync(It.IsAny<Tag>()), Times.Once);
        }

        [TestMethod]
        public async Task Create_Tag_Ok()
        {
            _mockTagRepository.Setup(x => x.CreateTagAsync(It.IsAny<Tag>()));
            _tagService = new TagService(_mockUnitOfWork.Object);
            int id = await _tagService.CreateTagAsync(new Tag() { Id = 1 });
            Assert.AreEqual(1, id);
            _mockUnitOfWork.Verify(x => x.TagRepository.CreateTagAsync(It.IsAny<Tag>()), Times.Once);
        }

        // Task<List<Tag>> GetAllTagsAsync()

        [TestMethod]
        public async Task Get_AllTags_Ok()
        {
            _mockUnitOfWork.Setup(x => x.TagRepository.GetAllTagsAsync()).Returns(Task.FromResult(new List<Tag>()));
            _tagService = new TagService(_mockUnitOfWork.Object);
            await _tagService.GetAllTagsAsync();
            _mockUnitOfWork.Verify(x => x.TagRepository.GetAllTagsAsync(), Times.Once);
        }

        // Task<List<Tag>> GetTagsByIdListAsync(List<int> tagsId)

        [TestMethod]
        public async Task Get_TagsByIdList_ReturnsNothing()
        {
            _mockUnitOfWork.Setup(x => x.TagRepository.GetTagsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(new List<Tag>()));
            _tagService = new TagService(_mockUnitOfWork.Object);

            await Assert.ThrowsExceptionAsync<OLNotFound>(() => _tagService.GetTagsByIdListAsync(new List<int>()));
            _mockUnitOfWork.Verify(x => x.TagRepository.GetTagsByIdListAsync(It.IsAny<List<int>>()), Times.Once);
        }

        [TestMethod]
        public async Task Get_TagsByIdList_OK()
        {
            _mockUnitOfWork.Setup(x => x.TagRepository.GetTagsByIdListAsync(It.IsAny<List<int>>())).Returns(Task.FromResult(new List<Tag>() { new Tag() }));
            _tagService = new TagService(_mockUnitOfWork.Object);

            List<Tag> result = await _tagService.GetTagsByIdListAsync(new List<int>());

            _mockUnitOfWork.Verify(x => x.TagRepository.GetTagsByIdListAsync(It.IsAny<List<int>>()), Times.Once);
        }
    }
}
