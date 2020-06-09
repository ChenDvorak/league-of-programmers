import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BlogDetail, BlogService } from '../../../services/blog.service';

@Component({
  selector: 'app-blog-detail',
  templateUrl: './blog-detail.component.html',
  styleUrls: ['./blog-detail.component.sass']
})
export class BlogDetailComponent implements OnInit {

  id: number;
  detail: BlogDetail = {
    title: '',
    targets: [],
    content: '',
    views: 0,
    dateTime: '',
    author: '',
    authorAccount: ''
  };

  constructor(
    private route: ActivatedRoute,
    private blogService: BlogService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(p => {
      this.id = +p.get('id');
      this.getDetail();
    });
  }

  private getDetail() {
    this.blogService.getBlogDetail(this.id).subscribe(resp => {
      if (resp.status === 200) {
        this.detail = resp.data;
      }
    });
  }
}
