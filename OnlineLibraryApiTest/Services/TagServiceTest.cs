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

        private Mock<IUnitOfWork> _mockUnitOfWork;

        private Mock<ITagRepository> _mockTagRepository;

        [TestInitialize]
        public void InitializeTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTagRepository = new Mock<ITagRepository>();
            _mockUnitOfWork.Setup(x => x.TagRepository).Returns(_mockTagRepository.Object);
        }

        [TestMethod]
        public async Task Get_AllTags_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.TagRepository.GetAllTagsAsync()).Returns(Task.FromResult(new List<Tag>() { }));
            _tagService = new TagService(_mockUnitOfWork.Object);

            List<Tag> tags = await _tagService.GetAllTagsAsync();
            Assert.IsFalse(tags.Any());
            _mockUnitOfWork.Verify(x => x.TagRepository.GetAllTagsAsync(), Times.Once);
        }

        [TestMethod]
        public async Task Get_AllTags_Ok()
        {
            List<Tag> tags = new List<Tag>() { new Tag() };
            _mockUnitOfWork.Setup(x => x.TagRepository.GetAllTagsAsync()).Returns(Task.FromResult(tags));
            _tagService = new TagService(_mockUnitOfWork.Object);
            List<Tag> result = await _tagService.GetAllTagsAsync();
            Assert.AreEqual(tags, result);
            _mockUnitOfWork.Verify(x => x.TagRepository.GetAllTagsAsync(), Times.Once);
        }

        [TestMethod]
        [DataRow(0, -2, 100)]
        [DataRow(1, -23)]
        [DataRow()]
        [ExpectedException(typeof(OLNotFound))]
        public async Task Get_TagsByIdList_ReturnsNothing(params int[] ids)
        {
            List<int> tagsId = ids.ToList();
            _mockUnitOfWork.Setup(x => x.TagRepository.GetTagsByIdListAsync(tagsId)).Returns(Task.FromResult(new List<Tag>() { }));
            _tagService = new TagService(_mockUnitOfWork.Object);

            await _tagService.GetTagsByIdListAsync(tagsId);
            _mockUnitOfWork.Verify(x => x.TagRepository.GetTagsByIdListAsync(tagsId), Times.Once);
        }

        [TestMethod]
        [DataRow(0, -2, 100)]
        [DataRow(1, -23)]
        [DataRow(1, 2, 3)]
        public async Task Get_TagsByIdList_OK(params int[] ids)
        {
            List<int> tagsId = ids.ToList();
            List<Tag> tags = new List<Tag>() { new Tag() };

            _mockUnitOfWork.Setup(x => x.TagRepository.GetTagsByIdListAsync(tagsId)).Returns(Task.FromResult(tags));
            _tagService = new TagService(_mockUnitOfWork.Object);

            List<Tag> result = await _tagService.GetTagsByIdListAsync(tagsId);

            Assert.AreEqual(tags, result);
            _mockUnitOfWork.Verify(x => x.TagRepository.GetTagsByIdListAsync(tagsId), Times.Once);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        [ExpectedException(typeof(OLBadRequest))]
        public async Task Create_Tag_FieldsIsIncorrect(string name)
        {
            _mockUnitOfWork.Setup(x => x.TagRepository.InsertTag(It.IsAny<Tag>()));
            _tagService = new TagService(_mockUnitOfWork.Object);
            await _tagService.CreateTagAsync(new Tag() { Name = name });
            _mockUnitOfWork.Verify(x => x.TagRepository.InsertTag(It.IsAny<Tag>()), Times.Once);
        }

        [TestMethod]
        [DataRow("s")]
        public async Task Create_Tag_OK(string name)
        {
            Tag tag = new Tag() { Id = 1, Name = name };
            _mockUnitOfWork.Setup(x => x.TagRepository.InsertTag(It.IsAny<Tag>()));
            _tagService = new TagService(_mockUnitOfWork.Object);
            int? id = await _tagService.CreateTagAsync(tag);

            _mockUnitOfWork.Verify(x => x.TagRepository.InsertTag(It.IsAny<Tag>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveAsync(), Times.Once);
        }
    }
}
