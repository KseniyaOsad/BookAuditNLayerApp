using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnlineLibrary.BLL.Services;
using OnlineLibrary.Common.DBEntities;
using OnlineLibrary.Common.Exceptions;
using OnlineLibrary.DAL.Interfaces;
using System.Collections.Generic;
using System.Linq;

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
        public void Get_AllTags_ListIsEmpty()
        {
            _mockUnitOfWork.Setup(x => x.TagRepository.GetAllTags()).Returns(new List<Tag>() { });
            _tagService = new TagService(_mockUnitOfWork.Object);

            List<Tag> tags = _tagService.GetAllTags();
            Assert.IsFalse(tags.Any());
            _mockUnitOfWork.Verify(x => x.TagRepository.GetAllTags(), Times.Once);
        }

        [TestMethod]
        public void Get_AllTags_Ok()
        {
            List<Tag> tags = new List<Tag>() { new Tag() };
            _mockUnitOfWork.Setup(x => x.TagRepository.GetAllTags()).Returns(tags);
            _tagService = new TagService(_mockUnitOfWork.Object);
            List<Tag> result = _tagService.GetAllTags();
            Assert.AreEqual(tags, result);
            _mockUnitOfWork.Verify(x => x.TagRepository.GetAllTags(), Times.Once);
        }

        [TestMethod]
        [DataRow(0, -2, 100)]
        [DataRow(1, -23)]
        [DataRow()]
        public void Get_TagsByIdList_ReturnsNothing(params int[] ids)
        {
            List<int> tagsId = ids.ToList();
            _mockUnitOfWork.Setup(x => x.TagRepository.GetTagsByIdList(tagsId)).Returns(new List<Tag>() { });
            _tagService = new TagService(_mockUnitOfWork.Object);

            Assert.ThrowsException<OLNotFound>(() => _tagService.GetTagsByIdList(tagsId), "Expected Exception");
            _mockUnitOfWork.Verify(x => x.TagRepository.GetTagsByIdList(tagsId), Times.Once);
        }

        [TestMethod]
        [DataRow(0, -2, 100)]
        [DataRow(1, -23)]
        [DataRow(1, 2, 3)]
        public void Get_TagsByIdList_OK(params int[] ids)
        {
            List<int> tagsId = ids.ToList();
            List<Tag> tags = new List<Tag>() { new Tag() };

            _mockUnitOfWork.Setup(x => x.TagRepository.GetTagsByIdList(tagsId)).Returns(tags);
            _tagService = new TagService(_mockUnitOfWork.Object);

            List<Tag> result = _tagService.GetTagsByIdList(tagsId);

            Assert.AreEqual(tags, result);
            _mockUnitOfWork.Verify(x => x.TagRepository.GetTagsByIdList(tagsId), Times.Once);
        }


        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void Create_Tag_FieldsIsIncorrect(string name)
        {
            _mockUnitOfWork.Setup(x => x.TagRepository.InsertTag(It.IsAny<Tag>()));
            _tagService = new TagService(_mockUnitOfWork.Object);
            Assert.ThrowsException<OLBadRequest>(() => _tagService.CreateTag(new Tag() { Name = name }), "Expected Exception");
            _mockUnitOfWork.Verify(x => x.TagRepository.InsertTag(It.IsAny<Tag>()), Times.Once);
        }

        [TestMethod]
        [DataRow("s")]
        public void Create_Tag_OK(string name)
        {
            Tag tag = new Tag() { Id = 1, Name = name };
            _mockUnitOfWork.Setup(x => x.TagRepository.InsertTag(It.IsAny<Tag>()));
            _tagService = new TagService(_mockUnitOfWork.Object);
            int? id = _tagService.CreateTag(tag);

            _mockUnitOfWork.Verify(x => x.TagRepository.InsertTag(It.IsAny<Tag>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Save(), Times.Once);
        }
    }
}
